#include <stm32f10x.h>




u16 GenerateCRITT(u8 *p)
        {
            u32 uiCRCITTSum = 0xFFFF;
            u32 uiByteValue;
            int iBufferIndex;
            int iBitIndex;

            for (iBufferIndex = 0;iBufferIndex < 64; iBufferIndex++)
            {
                uiByteValue = ((u32)p[iBufferIndex] << 8);
                for (iBitIndex = 0; iBitIndex < 8; iBitIndex++)
                {
                    if (((uiCRCITTSum ^ uiByteValue) & 0x8000) != 0)
                    {
                        uiCRCITTSum = (uiCRCITTSum << 1) ^ 0x1021;
                    }
                    else
                    {
                        uiCRCITTSum <<= 1;
                    }
                    uiByteValue <<= 1;
                }
            }
            return (u16)uiCRCITTSum;
        }
