namespace DotnetApi.Dtos
{
    public partial class UserSalaryToAddDto
    {
        public int UserId {get; set;}
        public decimal Salary {get; set;} = 0m;
    }
}
