#include "stm32f10x.h"
#include "NRF2401L.h"
#include "exit.h"
#include "PWM.h"
#include "SYSTEM.h"


u8 Rx_Buf[96];
u8 Rx_Flag_Over=0;   //接受完成标志
u8 System_Check=0;  //接受完成标志


u8 num=0;//////////////////////////////////////////////////////////////




int main()
{
	NRF24L01_Init();
	EXIT_Init();
	TIM3_Init();
	TIM4_Init();
	while(NRF24L01_Check());//检测NRF2401L是否存在
	NRF24L01_RX_Mode();    //接受模式
	
	
	while(1)
	{
		if(Rx_Flag_Over==1)
		{
			Rx_Flag_Over=0;
			switch(Rx_Buf[10])
			{
				case 0x00:
				{
					Reset_Duoji();
					NRF24L01_TX_Mode();
					delay_us(100);
					NRF24L01_TxPacket(System_Status_Connecting);
					//delay_ms(100);
					//NRF24L01_TxPacket(System_Status_Normal);
					break; 
				}	
				
				case 0xf0:
				{
					Reset_Duoji();
					break;  //停止信号，复位舵机
				}
				
				case 0x0f:
				{
					Control_Duoji(Rx_Buf);
					NRF24L01_TX_Mode();
					delay_us(100);
					if(System_Check==1)
					{
					 NRF24L01_TxPacket(System_Status_Working);//舵机程序运行正常，发送工作正常信号
					 System_Check=0;
					}
					else
					{
						NRF24L01_TxPacket(System_Status_Abnormal);//舵机程序运行不正常，发送系统异常信号
						Reset_Duoji();
          }
					break;
				}
					
				default:NRF24L01_TxPacket(System_Status_Abnormal);break;/////////////////////////////		
      }	
			
			NRF24L01_RX_Mode();    //接受模式	,接受下一帧数据包	
    }
			
  }
	
}
