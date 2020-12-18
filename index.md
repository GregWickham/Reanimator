![Reanimator gloss](/images/ReanimatorGloss.jpg)

**The world is full of legacy software, running all types of businesses in all types of industries.**

In our personal lives we're all accustomed to smartphones, tablets, and cloud services; but if you step behind the counter in one of those businesses and look at the dusty 14 inch CRT used by the hourly employees, chances are you'll see something like this:

![WDS-II screen capture](/images/WDS_II_example.jpg)

Sure, all those employees have smartphones for their personal use.  They have other computers in the back office, running web-enabled Windows applications.  Yet the software that tracks their sales, inventory, accounting and payroll may not have changed in 20 or 30 years.

**But why?**  Why haven't they ripped that out and replaced it with something modern?

The business owner probably chose that software package decades ago when it was state-of-the-art.  The business has grown up around that software.  The staff are trained and experienced in its use; and it's tightly integrated into their supply chain.  Most important, that software contains the mission-critical data that keeps the business running.  Replacing it would be costly and disruptive.  To the owner, ripping out that "obsolete" software and replacing it looks like a huge, expensive headache.

At the same time, the owner is well aware that technology has moved on.  It would be great if they could integrate that old software with other, modern systems -- Ecommerce, mobile devices, web services, etc -- but they just don't know how.

# Reanimator

What if that business had a software adapter layer that could operate the user interface of their old software, as if the old software had a rich developer API -- extracting data and exporting it to other systems, performing the same functions that employees do when they use the software?  That's what **Reanimator** does.

You can use that ability to put new GUI skins on top of old functionalty, changing workflow or just updating the look and feel.  You can add service endpoints to the old software, so its functionalty can be accessed from web portals, mobile devices, or whatever.  You can mine the data in the old system, and present it to users in a new way.

Here are some examples of applications I've done with a legacy system called WDS-II:

### Point of Sale

[![Point of Sale](http://img.youtube.com/vi/3t3N_ouGeCk/0.jpg)](http://www.youtube.com/watch?v=3t3N_ouGeCk "Point of Sale")

### Counting inventory with speech recognition

[![Counting Inventory](http://img.youtube.com/vi/ziy9DvCxrRc/0.jpg)](http://www.youtube.com/watch?v=ziy9DvCxrRc "Counting Inventory")

### Managing Stock Lists

[![Managing Stock Lists](http://img.youtube.com/vi/2bcLmyh1NrA/0.jpg)](http://www.youtube.com/watch?v=2bcLmyh1NrA "Managing Stock Lists")

### Product Lookup From a Mobile Device

[![Product Lookup From a Mobile Device](http://img.youtube.com/vi/tah2IuiGp5Q/0.jpg)](http://www.youtube.com/watch?v=tah2IuiGp5Q "Product Lookup From a Mobile Device")

# How Does It Work?

In the discussion that follows, I'll refer to the legacy application as a "console application."  This type of application has a non-GUI, character based user interface on a screen that's most often 80 x 24 characters, although other resolutions are possible.

To integrate with a particular console application, there are two major steps:

1.  Build a static model of the console application's user interface.  You don't need to model the **entire** user interface -- just the parts containig the functionality with which you want to integrate.

2.  Define one or more **Activities** that exercise the relevant parts of the console application's user interface, to accomplish the desired task.

The [Reanimator GitHub repo](https://github.com/GregWickham/Reanimator) contains two C# code files that illustrate how this is done.

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
... describes the screen as a state machine.  The states represent points in the WDS-II user interface at which the software is waiting for user input; I call such a state the "focus" of the console application.   The transitions in this state machine are triggered by user input events, which can be a keypress or a typed string terminated by a CR/LF.  For example, this declaration:
```        
        MenuFor_AR_Inquiries
          .OnKey(Keys.P, PaymentsAndAdjustments.Menu)
          .OnKey(Keys.I, Invoice.ResponseTo_ShowInvoices)
          .OnKey(Keys.N, PromptFor_CustomerID);
```   
... describes a menu in the WDS-II user interface that can take three user inputs:

* A keypress 'P' that causes a transition to the `PaymentsAndAdjustments.Menu`;
* A keypress 'I' that causes a transition to the `Invoice.ResponseTo_ShowInvoices`; or
* A keypress 'N' that causes a transition to the `PromptFor_CustomerID`.
    
