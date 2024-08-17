using AutorizationMicroService.Models;

namespace AutorizationMicroService.TwoFactorAutorization
{
    public struct PhoneMemberCode
    {
        private int Code = 0;
        private DateTime DateOfGenerate = new DateTime();

        public PhoneMemberCode(int code, DateTime dateOfGenerate)
        {
            this.Code = code;
            this.DateOfGenerate = dateOfGenerate;
        }

        public void SetCode(int code) { 
            this.Code = code;
        }

        public void SetLastDate(DateTime date)
        {
            this.DateOfGenerate = date;
        }

        public int ReturnCode()
        { return this.Code; }
    }

    public interface ITwoAutorizer
    {
        public Dictionary<string, PhoneMemberCode> TwoFactoryMemory { get; }

        public void GenerateCode(string phoneNumber, Random rnd);

        public bool EqualseCode(string phoneNumber, int code);

        public int GetCode(string phoneNumber);

    }
}
