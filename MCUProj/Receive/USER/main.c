#include "stm32f10x.h"
#include "NRF2401L.h"
#include "exit.h"

u8 RxBuf[32];
u8 sta1[24];
u8 count;
u8 flag=0;
u32 count1=0;


int main()
{
	NRF24L01_Init();
	EXIT_Init();
	while(NRF24L01_Check());//ºÏ≤‚NRF2401L «∑Ò¥Ê‘⁄
	NRF24L01_RX_Mode();

	
 
	while(1)
	{
		/*if(flag==0)
		{
			for(count=0;count<24;count++)
		  {
				sta1[count]=NRF24L01_Read_Reg(count);
      }
	
    }*/
   	
		//NRF24L01_RxPacket(RxBuf);
		
		
  }
	
}
