#include "stm32f10x.h"
#include "NRF2401L.h"
#include "exit.h"
#include "PWM.h"

u8 Rx_Buf[96];
u8 count;
//u8 Control_Buf[96];

int main()
{
	NRF24L01_Init();
	EXIT_Init();
	TIM3_Init();
	TIM4_Init();
	while(NRF24L01_Check());//ºÏ≤‚NRF2401L «∑Ò¥Ê‘⁄
	NRF24L01_RX_Mode();
	
	
	while(1)
	{
		if(count==0)
		{
			switch(Rx_Buf[10])
			{
				case 0x00:Reset_Duoji();break;
				case 0xf0:Reset_Duoji();break;
				case 0x0f:Control_Duoji(Rx_Buf);break;
				default:  break;
      }
			
    }
		
  }
	
}
