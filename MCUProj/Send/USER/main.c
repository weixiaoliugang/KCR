#include "stm32f10x.h"
#include "usart.h"
#include "nrf2401l.h"

u8 Rx_Over_Flag=0;
u8 Rx_Buf[64];//���ܵ����ݰ�
//u8 Rx_Buf1[64];//�������ݰ�
u8 Tx_Buf[64];//���ڷ��͵����ݰ�

int main()
{
	USART1_Config(115200);//��������
	NRF24L01_Init();
	while(NRF24L01_Check());
	NRF24L01_TX_Mode();
		
	while(1)
	{
		if(Rx_Over_Flag==1)
		{
			switch(Rx_Buf[10])//����ʮһλ��ʲô�ź�
			{
				case 0x00:;     //����
        case 0x0f:;      //��ʼ
        case 0xf0:;      //ֹͣ
      }
			
    }
  }
	
}
