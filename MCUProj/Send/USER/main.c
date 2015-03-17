#include "stm32f10x.h"
#include "usart.h"
#include "nrf2401l.h"
#include "EXIT.h"
#include "SYSTEM.h"

u8 Tx_Buf[96];         //���ڷ��͵����ݰ�
u8 Tx_Flag_Over=0;     //���ܽ�����־λ

int main()
{
	USART1_Config(115200);//��������
	NRF24L01_Init();
	EXIT_Init();
	while(NRF24L01_Check());
	NRF24L01_TX_Mode(); //����Ϊ����ģʽ

	while(1)
	{
		if(Tx_Flag_Over==1)
		{
			
			NRF24L01_TX_Mode();   //�յ����ݰ��ı�Ϊ����ģʽ
			Tx_Flag_Over=0;
											
		  NRF24L01_TxPacket(Tx_Buf);
			delay_us(100);//������δ����
			NRF24L01_TxPacket(Tx_Buf+32);
			delay_us(100);//����δ����
		  NRF24L01_TxPacket(Tx_Buf+64);
			NRF24L01_RX_Mode();//����Ϊ����ģʽ
																			 
		}
    		
	}
	
}
