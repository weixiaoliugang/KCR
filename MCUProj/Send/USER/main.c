#include "stm32f10x.h"
#include "usart.h"
#include "nrf2401l.h"

u8 Tx_Buf[96];         //用于发送的数据包
u8 Tx_Flag_Over=0;     //接受结束标志位


int main()
{
	USART1_Config(115200);//串口配置
	NRF24L01_Init();
	while(NRF24L01_Check());
	NRF24L01_TX_Mode();
		
	while(1)
	{
		if(Tx_Flag_Over==1)
		{
			Tx_Flag_Over=0;
			switch(Tx_Buf[10])
			{
				case 0x00:NRF24L01_TxPacket(Tx_Buf);NRF24L01_TxPacket(Tx_Buf+32);NRF24L01_TxPacket(Tx_Buf+64);break;
				case 0x0f:NRF24L01_TxPacket(Tx_Buf);NRF24L01_TxPacket(Tx_Buf+32);NRF24L01_TxPacket(Tx_Buf+64);break;
				case 0xf0:NRF24L01_TxPacket(Tx_Buf);NRF24L01_TxPacket(Tx_Buf+32);NRF24L01_TxPacket(Tx_Buf+64);break;
				default:break; 
      }
			
			
			
    }
	}
	
}
