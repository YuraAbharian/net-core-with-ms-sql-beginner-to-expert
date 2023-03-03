namespace DotnetApi.Dtos
{
    public partial class UserForLoginConfirmationDto
    {
        public byte[] PasswordSalt  {get;set;} = new byte[0];
        public byte[] PasswordHash  {get;set;} = new byte[0];
    }
}
