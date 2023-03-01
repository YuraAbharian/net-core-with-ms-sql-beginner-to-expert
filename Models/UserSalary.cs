namespace DotnetApi.Models
{
    public partial class UserSalary
    {
        public int UserId {get; set;}
        public decimal Salary {get; set;} = 0;
        public decimal Average {get; set;} = 0;
    }
}
