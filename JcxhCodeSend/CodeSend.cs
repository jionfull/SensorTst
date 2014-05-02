using System;
using System.Collections.Generic;
using System.Text;
using Lon.IO.Ports;

namespace JcxhCodeSend
{
    public interface ICodeSend
    {
        bool SetFreq(float freq);
        bool SetFreq(int chNo,float freq);
        bool SetAmpli(float ampli);
        bool SetAmpli(int chNo,float ampli);

    }
    public class CodeSend3022 : ICodeSend
    {
        FunGen dev = new FunGen();

        public bool SetFreq(float freq)
        {
            bool ret = true;
            ret = dev.SetFreq(0, freq);
            if (ret == false)
            {
                return ret;
            }
            if (freq > 1600)
            {
                ret = dev.SetFmDevia(0, 11);
                if (ret == false)
                {
                    return ret;
                }
                dev.SetFmFreq(0, 10.3f);
                if (ret == false)
                {
                    return ret;
                }
            }
            else
            {
                dev.SetFmDevia(0, 55);
                if (ret == false)
                {
                    return ret;
                }
                dev.SetFmFreq(0, 8.5f);
                if (ret == false)
                {
                    return ret;
                }
            }
            return true;
        }
        public bool SetFreq(int chNo,float freq)
        {
            bool ret = true;
            ret = dev.SetFreq(chNo, freq);
            if (ret == false)
            {
                return ret;
            }
            if (freq > 1600)
            {
                ret = dev.SetFmDevia(chNo, 11);
                if (ret == false)
                {
                    return ret;
                }
                dev.SetFmFreq(chNo, 10.3f);
                if (ret == false)
                {
                    return ret;
                }
            }
            else
            {
                dev.SetFmDevia(chNo, 55);
                if (ret == false)
                {
                    return ret;
                }
                dev.SetFmFreq(chNo, 8.5f);
                if (ret == false)
                {
                    return ret;
                }
            }
            return true;
        }
        public bool SetAmpli(float ampli)
        {
        return dev.SetAmpli(0, ampli);
       }

        public bool SetAmpli(int chNo,float ampli)
        {
            return dev.SetAmpli(chNo, ampli);
        }
    }
}
