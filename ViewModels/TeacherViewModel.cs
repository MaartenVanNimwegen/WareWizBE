using System.IdentityModel.Tokens.Jwt;

namespace WareWiz.ViewModels
{
    public class TeacherViewModel : UserBase
    {
        public string jwtToken { get; set; }
    }
}