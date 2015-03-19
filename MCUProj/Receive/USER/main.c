#include "stm32f10x.h"
#include "NRF2401L.h"
#include "exit.h"
#include "PWM.h"
#include "SYSTEM.h"
#include "CRC.h"

u8 Rx_Buf[96];
u8 Rx_Buf_Bak[96];

    //接受完成标志
u8 Rx_Flag_Over=0;     
u8 Is_System_Normal=0;    //系统检查标志
u16 *CRC_Value;
u8 num;



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
            CRC_Value=(u16*)(Rx_Buf_Bak+76);
            if(*CRC_Value==GenerateCRITT(Rx_Buf_Bak+12,64))    //进行CRC校验
            {    
                switch(Rx_Buf_Bak[10])
                { 
                    case 0x00:  //停止信号，复位舵机
                    {
                        Reset_Duoji();
                        NRF24L01_TX_Mode();
                        delay_us(100);
                        NRF24L01_TxPacket(System_Status_Connecting);
                        break; 
                    }	
                    
                    case 0xf0://停止信号，复位舵机
                    {
                        Reset_Duoji();
                        NRF24L01_TX_Mode();
                        delay_us(100);
                        NRF24L01_TxPacket(System_Status_Normal);
                        break;  
                    }
                    
                    case 0x0f:    //停止信号，复位舵机
                    {
                        Control_Duoji(Rx_Buf_Bak);
                        NRF24L01_TX_Mode();
                        delay_us(100);
                        if(Is_System_Normal)
                        {
                            NRF24L01_TxPacket(System_Status_Working);
                            Is_System_Normal=0;
                        }
                        else
                        {
                            NRF24L01_TxPacket(System_Status_Abnormal);
                        }
                        break;
                    }
                    
                    default:
                    {
                        NRF24L01_TX_Mode();
                        delay_us(100);
                        NRF24L01_TxPacket(System_Status_Normal);
                    }	
               }	
               NRF24L01_RX_Mode();    //接受模式	,接受下一帧数据包
           }               
       }		
    }
	
}
