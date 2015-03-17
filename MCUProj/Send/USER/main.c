#include "stm32f10x.h"
#include "usart.h"
#include "nrf2401l.h"
#include "EXIT.h"
#include "SYSTEM.h"

u8 Tx_Buf[96];         //用于发送的数据包
u8 Tx_Flag_Over=0;     //接受结束标志位

int main()
{
	USART1_Config(115200);//串口配置
	NRF24L01_Init();
	EXIT_Init();
	while(NRF24L01_Check());
	NRF24L01_TX_Mode(); //设置为发送模式

	while(1)
	{
		if(Tx_Flag_Over==1)
		{
			
			NRF24L01_TX_Mode();   //收到数据包改变为发送模式
			Tx_Flag_Over=0;
											
		  NRF24L01_TxPacket(Tx_Buf);
			delay_us(100);//保留，未处理
			NRF24L01_TxPacket(Tx_Buf+32);
			delay_us(100);//保留未处理
		  NRF24L01_TxPacket(Tx_Buf+64);
			NRF24L01_RX_Mode();//设置为接受模式
																			 
		}
    		
	}
	
}
