using System.Drawing;
using Keys = System.Windows.Forms.Keys;
using Reanimator.Elements;
using ConsoleApplication.UserInterface.Elements;

namespace SIMS.UserInterface.AccountsReceivable
{
    [Container("5-3")]
    public static class Inquiry_1 
    {
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

        public static readonly ConsoleDataFrame CustomerFrame = new ConsoleDataFrame
        {
            ScreenOrigin = new Point(0, 1),
            Description = c => c.ValueOf<string>("Name")
        }
            .AddField("Customer_Number", new StringField(10), new Point(7, 0))
            .AddField("Name", new StringField(36), new Point(17, 0))
            .AddField("Balance", new DecimalField(13), new Point(66, 0));

        [Container("5-3-I")]
        public static class Invoice
        {
            static Invoice()
            {
                Menu.Signature = Display.Input("Next").FollowedBy(SignatureOf.Menu.Strip);
                Message_NoActiveInvoices.Signature = Display.Input("No").FollowedBy("History").FollowedBy(SignatureOf.Prompt.To.PressSpaceBarToContinue);
                Message_EndOfInvoices.Signature = Display.Input("invoices").FollowedBy(SignatureOf.Prompt.To.PressSpaceBarToContinue);
                PromptFor_ContinueWithItems.Signature = SignatureOf.Prompt.To.PressSpaceBarToContinue;

                Menu
                    .OnKey(Keys.N, ResponseTo_NextPageOfInvoices)
                    .OnKey(Keys.E, MenuFor_AR_Inquiries);
                Message_NoActiveInvoices
                    .AutoProceedWith(Keys.Space, Menu);
                Message_EndOfInvoices
                    .OnKey(Keys.Space, Menu);
                ResponseTo_ShowInvoices
                    .Add(Menu)
                    .Add(Message_NoActiveInvoices);
                ResponseTo_NextPageOfInvoices
                    .Add(Menu)
                    .Add(Message_EndOfInvoices);
            }

            public static readonly Menu Menu = new Menu();
            public static readonly Message Message_NoActiveInvoices = new Message();
            public static readonly Selector ResponseTo_ShowInvoices = new Selector();
            public static readonly Message Message_EndOfInvoices = new Message();
            public static readonly Prompt PromptFor_ContinueWithItems = new Prompt();
            public static readonly Selector ResponseTo_NextPageOfInvoices = new Selector();

            public static readonly ConsoleListFrame InvoicesFrame = new ConsoleListFrame
            {
                ScreenOrigin = new Point(0, 10),
                NumberOfItems = 10,
                LinesPerItem = 1
            }
                .AddItemField<StringField>("Invoice_Number", 12, new Point(2, 0))
                .AddItemField<StringField>("PO_or_Check_Number", 14, new Point(15, 0))
                .AddItemField<DateField>("Date", 8, new Point(30, 0))
                .AddItemField<StringField>("Terms", 4, new Point(39, 0))
                .AddItemField<DateField>("Due_Date", 8, new Point(44, 0))
                .AddItemField<DecimalField>("Amount", 9, new Point(55, 0))
                .AddItemField<DecimalField>("Balance_Due", 9, new Point(67, 0));

            public static readonly ConsoleListFrame LineItemsFrame = new ConsoleListFrame
            {
                ScreenOrigin = new Point(0, 14),
                NumberOfItems = 6,
                LinesPerItem = 1
            }
                .AddItemField<IntegerField>("Line_Number", 3, new Point(1, 0))
                .AddItemField<StringField>("Part_Number", 12, new Point(5, 0))
                .AddItemField<StringField>("Description", 20, new Point(20, 0))
                .AddItemField<FloatField>("Quantity_Ordered", 5, new Point(44, 0))
                .AddItemField<FloatField>("Quantity_Shipped", 5, new Point(56, 0))
                .AddItemField<FloatField>("Amount", 8, new Point(68, 0));
        }

        [Container("5-3-P")]
        public static class PaymentsAndAdjustments
        {
            static PaymentsAndAdjustments()
            {
                Menu.Signature = Display.Input("Unapp").FollowedBy(SignatureOf.Menu.Strip);
                Message_EndOfPayments.Signature = Display.Input("End").FollowedBy("selection").FollowedBy(SignatureOf.Prompt.To.PressSpaceBarToContinue);
                Message_BeginningAtStartOfFile.Signature = Display.Input("Beginning").FollowedBy("selection").FollowedBy(SignatureOf.Prompt.To.PressSpaceBarToContinue);

                Menu
                    .OnKey(Keys.N, ResponseTo_NextPageOfPayments)
                    .OnKey(Keys.E, MenuFor_AR_Inquiries);
                Message_EndOfPayments
                    .OnKey(Keys.Space, Message_BeginningAtStartOfFile);
                Message_BeginningAtStartOfFile
                    .AutoProceedWith(Keys.Space, Menu);
                ResponseTo_NextPageOfPayments
                    .Add(Menu)
                    .Add(Message_EndOfPayments);
            }

            public static readonly Menu Menu = new Menu();
            public static readonly Message Message_EndOfPayments = new Message();
            public static readonly Selector ResponseTo_NextPageOfPayments = new Selector();
            public static readonly Message Message_BeginningAtStartOfFile = new Message();

            public static readonly ConsoleListFrame PaymentsFrame = new ConsoleListFrame
            {
                ScreenOrigin = new Point(0, 11),
                NumberOfItems = 9,
                LinesPerItem = 1
            }
                .AddItemField<StringField>("Item_Number", 15, new Point(2, 0))
                .AddItemField<DateField>("Date", 8, new Point(18, 0))
                .AddItemField<DecimalField>("Amount", 11, new Point(28, 0))
                .AddItemField<StringField>("Description", 26, new Point(41, 0))
                .AddItemField<DecimalField>("Balance_Due", 10, new Point(66, 0));
        }
    }

    [Container("5-4")]
    public static class Inquiry_2 
    {
        static Inquiry_2()
        {
            Message_ProgramPath.Signature = Display.Input("ar/ar920.p");
            PromptFor_CustomerNumber.Signature = OnScreen.Text("Enter customer number, ? for lookup, F4 to quit").At(0, 23);

            Message_ProgramPath
                .AutoProceedWith(Keys.Space, PromptFor_CustomerNumber);
            PromptFor_CustomerNumber
                .OnKey(Keys.F4, Unwind.ToJustBefore(Message_ProgramPath));
        }

        public static readonly Message Message_ProgramPath = new Message();
        public static readonly Prompt PromptFor_CustomerNumber = new Prompt();
    }
}
