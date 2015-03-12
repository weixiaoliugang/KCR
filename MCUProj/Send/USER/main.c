#include "stm32f10x.h"
#include "usart.h"
#include "nrf2401l.h"

u8 Rx_Over_Flag=0;
u8 Rx_Buf[64];//接受的数据包
//u8 Rx_Buf1[64];//备份数据包
u8 Tx_Buf[64];//用于发送的数据包

int main()
{
	USART1_Config(115200);//串口配置
	NRF24L01_Init();
	while(NRF24L01_Check());
	NRF24L01_TX_Mode();
		
	while(1)
	{
		if(Rx_Over_Flag==1)
		{
			switch(Rx_Buf[10])//检查第十一位是什么信号
			{
				case 0x00:;     //请求
        case 0x0f:;      //开始
        case 0xf0:;      //停止
      }
			
    }
  }
	
}
