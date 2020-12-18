using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleApplication.UserInterface.Elements;
using ConsoleApplication.UserInterface.Activities;
using SIMS.UserInterface.AccountsReceivable;

namespace SIMS.UserInterface.Activities
{
    public delegate void AR_Inquiry_AdjustedInvoiceLookup_EventHandler(string invoiceNumber, DateTime date, decimal amount, decimal balanceDue);
    public delegate void AR_Inquiry_AdjustmentLookup_EventHandler(string referenceNumber, DateTime date, decimal amount, string description, decimal balanceDue);
    public delegate void AR_Inquiry_InvoiceSeparator_EventHandler();
    public delegate void AR_Inquiry_InvoiceLookup_EventHandler(string invoiceNumber, string comments, DateTime date, string terms, DateTime dueDate, decimal amount, decimal balanceDue);

    public sealed class AccountsReceivableInquiry : LocalActivity
    {
        public AccountsReceivableInquiry() : base() => ActivityDescription = "A/R Inquiry";
        private Session inquirySession;
        private ConsoleDataFrame.Context customer;
        public string CustomerName => customer.ValueOf<string>("Name");
        public decimal AR_Balance => customer.ValueOf<decimal>("Balance");
        private StringField.Context customerNumber;
        private ConsoleListFrame.Context paymentAndAdjustment;
        private ConsoleListFrame.Context invoices;

        public Task StartUp() => Task.Run(async () =>
        {
            inquirySession = await Session.GetAvailable(ActivityDescription);
            inquirySession.NavigateTo(Inquiry_1.PromptFor_CustomerID);
            customer = Inquiry_1.CustomerFrame.In(inquirySession);
            customerNumber = customer.StringFieldNamed("Customer_Number");
            paymentAndAdjustment = Inquiry_1.PaymentsAndAdjustments.PaymentsFrame.In(inquirySession);
            invoices = Inquiry_1.Invoice.InvoicesFrame.In(inquirySession);
        });

        public sealed override Task Quit() => Task.Run(() => inquirySession.Release());

        public Task<decimal> LookupBalanceForCustomer(string customerID) => MutexTask(() =>
        {
            GoToCustomer(customerID);
            return (decimal)AR_Balance;
        });

        public Task LookupPaymentHistoryForCustomerSinceDate(string customerID, DateTime startDate) => Task.Run(() =>
        {
            bool startDateReached = false;
            bool endReached = false;
            GoToCustomer(customerID);
            Inquiry_1.MenuFor_AR_Inquiries.In(inquirySession).Press(Keys.P);
            do
            {
                foreach (ConsoleDataFrame.Context item in paymentAndAdjustment.Items)
                {
                    string itemNumber = item.ValueOf<string>("Item_Number");
                    if (itemNumber == null)
                    {   // It's a separator
                        OnInvoiceSeparator();
                    }
                    else
                    {   // It's either an invoice or an adjustment.  All the fields are the same in either case, EXCEPT for the item number
                        DateTime date = item.ValueOf<DateTime>("Date");
                        if (date < startDate)
                        {
                            startDateReached = true;
                            break;
                        }

                        decimal amount = item.ValueOf<decimal>("Amount");
                        decimal balanceDue = item.ValueOf<decimal>("Balance_Due");

                        if (itemNumber.StartsWith("  "))
                        {   // It's an adjustment                                
                            string referenceNumber = itemNumber.TrimStart();
                            string description = item.ValueOf<string>("Description");
                            OnAdjustmentLookedUp(referenceNumber, date, amount, description, balanceDue);
                        }
                        else
                        {   // It's an invoice   
                            string invoiceNumber = itemNumber;
                            OnAdjustedInvoiceLookedUp(invoiceNumber, date, amount, balanceDue);
                        }
                    }
                }
                Inquiry_1.PaymentsAndAdjustments.Menu.In(inquirySession).Choose(Keys.N);
                if (inquirySession.Focus == Inquiry_1.PaymentsAndAdjustments.Message_EndOfPayments)
                {
                    endReached = true;
                    Inquiry_1.PaymentsAndAdjustments.Message_EndOfPayments.In(inquirySession).Press(Keys.Space);
                }
            } while (!(startDateReached || endReached));
            Inquiry_1.PaymentsAndAdjustments.Menu.In(inquirySession).Choose(Keys.E);         // Back out of the Payments and Adjustments lookup frame     
        });

        public Task LookupInvoicesForCustomer(string customerID) => Task.Run(() =>
        {
            bool endReached = false;
            GoToCustomer(customerID);
            Inquiry_1.MenuFor_AR_Inquiries.In(inquirySession).Choose(Keys.I);
            do
            {
                foreach (ConsoleDataFrame.Context invoice in invoices.Items)
                {
                    string comments = invoice.ValueOf<string>("PO_or_Check_Number");
                    if ((comments != null) && (comments.Trim().Equals("*** END ***")))
                    {
                        endReached = true;
                        break;
                    }
                    else
                    {
                        string invoiceNumber = invoice.ValueOf<string>("Invoice_Number");
                        DateTime date = invoice.ValueOf<DateTime>("Date");
                        string terms = invoice.ValueOf<string>("Terms");
                        DateTime dueDate = invoice.ValueOf<DateTime>("Due_Date");
                        decimal amount = invoice.ValueOf<decimal>("Amount");
                        decimal balanceDue = invoice.ValueOf<decimal>("Balance_Due");
                        OnInvoiceLookedUp(invoiceNumber, comments, date, terms, dueDate, amount, balanceDue);
                    }                        
                }
                if (!endReached)
                {
                    Inquiry_1.Invoice.Menu.In(inquirySession).Choose(Keys.N);
                    if (inquirySession.Focus == Inquiry_1.Invoice.Message_EndOfInvoices)
                    {
                        endReached = true;
                        Inquiry_1.Invoice.Message_EndOfInvoices.In(inquirySession).Press(Keys.Space);
                    }
                }
            } while (!endReached);
            Inquiry_1.Invoice.Menu.In(inquirySession).Choose(Keys.E);
        });
 
        private void GoToCustomer(string customerID)
        {
            inquirySession.NavigateTo(Inquiry_1.PromptFor_CustomerID);
            Inquiry_1.PromptFor_CustomerID.In(inquirySession).Respond(customerID);
            inquirySession.ItemDescription = CustomerName;
        }

        #region Result reporting

        public event AR_Inquiry_AdjustedInvoiceLookup_EventHandler AdjustedInvoiceLookedUp;
        private void OnAdjustedInvoiceLookedUp(string invoiceNumber, DateTime date, decimal amount, decimal balanceDue)
        {
            AdjustedInvoiceLookedUp?.Invoke(invoiceNumber, date, amount, balanceDue);
        }

        public event AR_Inquiry_AdjustmentLookup_EventHandler AdjustmentLookedUp;
        private void OnAdjustmentLookedUp(string referenceNumber, DateTime date, decimal amount, string description, decimal balanceDue)
        {
            AdjustmentLookedUp?.Invoke(referenceNumber, date, amount, description, balanceDue);
        }

        public event AR_Inquiry_InvoiceSeparator_EventHandler InvoiceSeparator;
        private void OnInvoiceSeparator()
        {
            InvoiceSeparator?.Invoke();
        }

        public event AR_Inquiry_InvoiceLookup_EventHandler InvoiceLookedUp;
        private void OnInvoiceLookedUp(string invoiceNumber, string comments, DateTime date, string terms, DateTime dueDate, decimal amount, decimal balanceDue)
        {
            InvoiceLookedUp?.Invoke(invoiceNumber, comments, date, terms, dueDate, amount, balanceDue);
        }

        #endregion

    }
}
