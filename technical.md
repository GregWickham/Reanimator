![Reanimator gloss](/images/ReanimatorGloss.jpg)

# How Does Reanimator Work?

In the technical discussion that follows, I'll refer to the legacy application as a "console application."  This type of application has a non-GUI, character-based user interface on a screen that's most often 80 x 24 characters, though other resolutions are possible.

To integrate with a particular console application, there are two major steps:

1.  Build a *static* model of the console application's user interface.  You don't need to model the **entire** user interface -- just the parts containing the functionality you want to use.

2.  Define one or more **Activities** that exercise the relevant parts of the console application's user interface, to accomplish the desired task.  Activities represent a *dynamic* model of the console application's user interface.  An Activity navigates through the static model by simulating user input events.

The [Reanimator GitHub repo](https://github.com/GregWickham/Reanimator) contains two C# code files that illustrate how this is done.

## The Static Model

The static model provides an abstract, *context-free* description of the console application's user interface.  Rather than describing what the user interface is actually doing at a moment in time, the static model describes what the user interface is hypothetically *capable* of doing.

The file called [03-04_Inquiry.cs](https://github.com/GregWickham/Reanimator/blob/master/03-04_Inquiry.cs) contains the static description of a WDS-II screen used for retrieving a customer's account information.  Here's what that screen looks like in WDS-II:

![WDS-II screen for Accounts Receivable Inquiry](/images/AR_Inquiry1_Screen.jpg)

This part of the static model:
```
    public static readonly ConsoleDataFrame CustomerFrame = new ConsoleDataFrame
    {
        ScreenOrigin = new Point(0, 1),
        Description = c => c.ValueOf<string>("Name")
    }
        .AddField("Customer_Number", new StringField(10), new Point(7, 0))
        .AddField("Name", new StringField(36), new Point(17, 0))
        .AddField("Balance", new DecimalField(13), new Point(66, 0));
```
... describes the fields on this screen from which we want to scrape data.  In this case the fields of interest are the ones containing the customer number, the customer name, and the customer's account balance.  The above description specifies where those fields can be found on the screen, the size of those fields, and what type of data will be found in those fields.

This part of the static model:
```
        static Inquiry_1()
        {
            Close = Unwind.ToJustBefore(Message_ProgramPath);
            Message_ProgramPath.Signature = Display.Input("ar/ar900.p");
            PromptFor_CustomerID.Signature = Display.Input("Cust.").FollowedBy("End.");
            MenuFor_AR_Inquiries.Signature = SignatureOf.Menu.Strip;

            Message_ProgramPath
                .AutoProceedWith(Keys.Space, PromptFor_CustomerID);
            PromptFor_CustomerID
                .OnKey(Keys.F4, Close)
                .OnResponse(e => MenuFor_AR_Inquiries);
            MenuFor_AR_Inquiries
                .OnKey(Keys.P, PaymentsAndAdjustments.Menu)
                .OnKey(Keys.I, Invoice.ResponseTo_ShowInvoices)
                .OnKey(Keys.N, PromptFor_CustomerID);
        }

        private static readonly Unwind Close;
        public static readonly Message Message_ProgramPath = new Message();
        public static readonly Prompt PromptFor_CustomerID = new Prompt();
        public static readonly Menu MenuFor_AR_Inquiries = new Menu();
```
... describes the screen's user interface behavior as a state machine.  The states represent points in the WDS-II user interface at which the console application is waiting for user input; I call such a state the current "focus" of the console application.  At any moment, the console application has at most one focus within a given session.

These declarations in the above snippet describe the focus states for the `Inquiry_1` screen:
```
        public static readonly Message Message_ProgramPath = new Message();
        public static readonly Prompt PromptFor_CustomerID = new Prompt();
        public static readonly Menu MenuFor_AR_Inquiries = new Menu();
```
These statements describe how to recognize when the user interface is in each focus state:
```
        Message_ProgramPath.Signature = Display.Input("ar/ar900.p");
        PromptFor_CustomerID.Signature = Display.Input("Cust.").FollowedBy("End.");
        MenuFor_AR_Inquiries.Signature = SignatureOf.Menu.Strip;
```
The `Signature` of a state describes state and / or events of the console application that Reanimator can use to recognize when the its user interface has reached a given focus.  In most cases this description is straightforward, but occasionally the console application can behave in ways that require some ingenuity to uniquely identify its state.  Having dealt with many of these irregular cases, I've developed a formidable toolbox of Signature elements.  Here's a selection of Signature examples to illustrate the variety of things that Reanimator can recognize:
```
        PromptFor_InvoiceDate.Signature = OnScreen.Text("Default for invoice date").At(1, 23);
```
```
        Question_ConfirmVoid.Signature = OnScreen.Text("Are you SURE you want to VOID this order ? :").At(15, 16)
            .FollowedBy(CursorIs.At(62, 16));
```
```
        PromptFor_InvoiceNumber.Signature = CursorIs.ToTheRightOf("INVOICE#:").By(2);
```
```
        PromptFor_LineNumber.Signature = OnScreen.Text("Enter Line #, 0 to find Item#, ? for Lookup, or F4 to End").At(0, 23)
            .FollowedBy(CursorIs.InField("Line_Number").OfListFrame(Orders.LineItems.LineItemsFrame));
```
```
        PromptFor_SelectLineItem.Signature = OnScreen.Text("Select Line Item, O for Options or F4 to cancel.").At(1, 23)
            .FollowedBy(OnScreen.ListFrame(Orders.LineItems.LineItemsFrame).HasBackgroundColor(Color.Green).InField("Line_Number"));
```
```
        PromptFor_CashAmount.Signature = OnScreen.ListFrame(TenderTypesFrame)
            .ItemWithStringValue("CASH").InField("Tender_Type").HasBackgroundColor(Color.Green).InField("Amount");
```
```
        PromptFor_ItemNumber_WithRedraw.Signature = Display.Input("Item#")
            .FollowedBy(OnScreen.Text("Enter Item# or press F2 for help explaining the other choices").At(0, 23))
            .FollowedBy(CursorMoved.ToStartOfField("Item_Number").InAnyItemOfListFrame(LineItemsFrame));
```
Handling the full variety of required Signatures, and providing a compact and expressive syntax for describing them, is one of the more difficult, innovative, and valuable aspects of the Reanimator framework.

Transitions in the Reanimator state machine are triggered by user input events, which can be:
* a keypress; or
* a typed string terminated by a CR/LF.  

For example, this declaration:
```        
        MenuFor_AR_Inquiries
          .OnKey(Keys.P, PaymentsAndAdjustments.Menu)
          .OnKey(Keys.I, Invoice.ResponseTo_ShowInvoices)
          .OnKey(Keys.N, PromptFor_CustomerID);
```   
... describes a menu in the WDS-II user interface that can take three user inputs:

* A keypress 'P' that causes a transition to the `PaymentsAndAdjustments.Menu`;
* A keypress 'I' that causes a transition to `Invoice.ResponseTo_ShowInvoices`; or
* A keypress 'N' that causes a transition to the `PromptFor_CustomerID`.
    
## The Dynamic Model

Once a static model is defined, we can create a dynamic model that brings it to life.  Every Activity takes place within the context of a `ConsoleApplicationSession`, which simulates a user connected to the console application through an ANSI terminal.

As we go through some of the code in [AccountsReceivableInquiry_1.cs](https://github.com/GregWickham/Reanimator/blob/master/AccountsReceivableInquiry_1.cs), you'll see the method `In(inquirySession)` several times.  This method is invoked on an element of the *static model*, and it returns an object representing that static element *in the context of a particular session.*  This provides a compact syntax for expressing actions and state not in the abstract, but *at a specific place and time.*

Most Activities implement a method called `StartUp()`: 
```
        public Task StartUp() => Task.Run(async () =>
        {
            inquirySession = await Session.GetAvailable(ActivityDescription);
            inquirySession.NavigateTo(Inquiry_1.PromptFor_CustomerID);
            customer = Inquiry_1.CustomerFrame.In(inquirySession);
            customerNumber = customer.StringFieldNamed("Customer_Number");
            paymentAndAdjustment = Inquiry_1.PaymentsAndAdjustments.PaymentsFrame.In(inquirySession);
            invoices = Inquiry_1.Invoice.InvoicesFrame.In(inquirySession);
        });
```
The first thing this `StartUp()` method does is obtain a `ConsoleApplicationSession` that will provide the context within which the Activity takes place:
```
        inquirySession = await Session.GetAvailable(ActivityDescription);
```
When a new `ConsoleApplicationSession` is created, that session is at the starting point of the console application user interface, so it must navigate through the menu structure of the user interface to reach the appropriate place for the Activity to do its work:
```
        inquirySession.NavigateTo(Inquiry_1.PromptFor_CustomerID);
```
Then we instantiate *in-context* versions of several user interface elements that we'll need, using the `In(inquirySession)` method:
```
        customer = Inquiry_1.CustomerFrame.In(inquirySession);
        customerNumber = customer.StringFieldNamed("Customer_Number");
        paymentAndAdjustment = Inquiry_1.PaymentsAndAdjustments.PaymentsFrame.In(inquirySession);
        invoices = Inquiry_1.Invoice.InvoicesFrame.In(inquirySession);
```
Once these *in-context* objects are available, we'll be able to query them to scrape data from the screen of the console application.

Now that the Activity is started up, it's ready to do some work.  The standard pattern is for an Activity to implement `public` methods with a return type of `Task` that expose its functionality to client code.  This pattern is not mandatory; it just happens to work well for most things.  The client that calls this exposed functionality could be anything -- a Windows user interface, a .NET API, a REST endpoint, whatever.
