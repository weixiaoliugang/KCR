#include "stm32f10x.h"
#include "NRF2401L.h"
#include "exit.h"
#include "PWM.h"

u8 Rx_Buf[96];
u8 Rx_Flag_Over=0;   //������ɱ�־
u8 System_Status[32];/////////////////////////////////////////////////////
u8 i;

int main()
{
	NRF24L01_Init();
	EXIT_Init();
	TIM3_Init();
	TIM4_Init();
	while(NRF24L01_Check());//���NRF2401L�Ƿ����
	NRF24L01_RX_Mode();    //����ģʽ
	for(i=0;i<32;i++)
	{
		System_Status[i]=0x00;
  }
	
	while(1)
	{
		if(Rx_Flag_Over==1)
		{
			Rx_Flag_Over=0;
			switch(Rx_Buf[10])
			{
				case 0x00:Reset_Duoji();NRF24L01_TX_Mode();NRF24L01_TxPacket(System_Status);break;    
				case 0xf0:Reset_Duoji();break;  //ֹͣ�źţ���λ���
				case 0x0f:Control_Duoji(Rx_Buf);NRF24L01_TX_Mode();NRF24L01_TxPacket(System_Status);break;
				default:break;
				
      }	
      NRF24L01_RX_Mode();    //����ģʽ	,������һ֡���ݰ�		
    }
		
  }
	
}
