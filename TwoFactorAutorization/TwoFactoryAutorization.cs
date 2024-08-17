using AutorizationMicroService.Models;

namespace AutorizationMicroService.TwoFactorAutorization
{
    public class TwoFactoryAutorization : ITwoAutorizer
    {
        public Dictionary<string, PhoneMemberCode> TwoFactoryMemory { get; set; }

        Random random;

        public TwoFactoryAutorization()
        {
            TwoFactoryMemory = new Dictionary<string, PhoneMemberCode>();
        }

        public bool EqualseCode(string phoneNumber, int code)
        {
            if(TwoFactoryMemory.ContainsKey(phoneNumber))
            {
                if (TwoFactoryMemory[phoneNumber].ReturnCode() == code)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public void GenerateCode(string phoneNumber, Random rnd)
        {
            if (TwoFactoryMemory.ContainsKey(phoneNumber))
            {
                TwoFactoryMemory[phoneNumber].SetCode(rnd.Next(1000, 10000));
            }
            else
            {
                PhoneMemberCode phoneMemberCode = new PhoneMemberCode();
                phoneMemberCode.SetCode(rnd.Next(1000, 10000));
                phoneMemberCode.SetLastDate(DateTime.Now);
                TwoFactoryMemory.TryAdd(phoneNumber, phoneMemberCode);
            }
        }

        public int GetCode(string phoneNumber)
        {
            return TwoFactoryMemory.ContainsKey(phoneNumber) ? TwoFactoryMemory[phoneNumber].ReturnCode() : 0;
        }
    }
}
