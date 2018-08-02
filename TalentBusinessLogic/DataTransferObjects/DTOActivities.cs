
using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects
{
    public class ActivityTemplate
    {
        public long TEMPLATE_ID { get; set; }
        public string NAME { get; set; }
    }

    public class ActivityStatus
    {
        public string DESCRIPTION { get; set; }
    }

    public class ActivityTemplateQA
    {
        public string TemplateID { get; set; }
        public string LoginID { get; set; }
        public string UserName { get; set; }
        public string ProductDescription1 { get; set; }
        public string ProductDescription2 { get; set; }
        public string ProductDescription3 { get; set; }
        public string ProductDescription4 { get; set; }
        public string ProductDescription5 { get; set; }
        public string Product { get; set; }
        public string Seat { get; set; }
        public string PriceBand { get; set; }
        public string BasketHeaderID { get; set; }
        public string FullName { get; set; }
        public string IsTemplatePerProduct { get; set; }
    }
    public class ActivityQuestionAnswer
    {
        public long TemplateID { get; set; }
        public long QuestionID { get; set; }
        public long Sequence { get; set; }
        public string QuestionText { get; set; }
        public int AnswerType { get; set; }
        public bool AllowSelectOtherOption { get; set; }
        public bool Mandatory { get; set; }
        public string PriceBandList { get; set; }
        public string RegularExpression { get; set; }
        public string HyperLink { get; set; }
        public bool RememberedAnswer { get; set; }
        public string Answer { get; set; }
        public Dictionary<long, string> ListOfAnswers { get; set; }
        public int TotalNumberOfSeats { get; set; }
        public int NumberOfQuestions { get; set; }
        public bool IsQuestionPerBooking { get; set; }
    }
}
