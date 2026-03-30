namespace WalletManagement.Core.DTOs
{
    public class SelfServiceFieldDTO
    {
        public string fieldName { get; set; }
        public string labelName { get; set; }
        public int fieldId { get; set; }
        public bool visibility { get; set; }
        public bool mandatory { get; set; }

    }
}
