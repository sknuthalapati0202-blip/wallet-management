namespace WalletManagement.Core.DTOs
{
    public class OrgCategoryFieldDetailsDTO
    {
        public string OrgCategoryName { get; set; }
        public int OrgCategoryId { get; set; }
        public string labelName { get; set; }
        public List<SelfServiceFieldDTO> organisationFieldDtos { get; set; }
    }
}
