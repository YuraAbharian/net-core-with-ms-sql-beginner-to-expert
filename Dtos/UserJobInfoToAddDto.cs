namespace DotnetApi.Dtos
{
    public partial class UserJobInfoToAddDto
    {
        public int UserId { get; set; }
        public string JobTitle { get; set; } = "";
        public string Department { get; set; } = "";
    }
}
