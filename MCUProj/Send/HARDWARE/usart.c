#include "stm32f10x.h"
#include "nrf2401l.h"



extern u8 Tx_Buf[96];
extern u8 Tx_Flag_Over;

void USART1_Config(u32  BaudRate )
{
	GPIO_InitTypeDef 	GPIO_InitStructure;
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef  NVIC_InitStructure;
	
  RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1|RCC_APB2Periph_GPIOA,ENABLE);
	
	USART_DeInit(USART1);
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_9;
	GPIO_InitStructure.GPIO_Mode=GPIO_Mode_AF_PP;
	GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
	GPIO_Init(GPIOA,&GPIO_InitStructure);
	
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_10;
	GPIO_InitStructure.GPIO_Mode=GPIO_Mode_IN_FLOATING;
	GPIO_Init(GPIOA,&GPIO_InitStructure);
	
	GPIO_PinRemapConfig(GPIO_Remap_USART1, ENABLE);//�򿪸��ù���
	
	USART_InitStructure.USART_BaudRate = BaudRate;//������Ϊ
	USART_InitStructure.USART_WordLength = USART_WordLength_8b; 
	USART_InitStructure.USART_StopBits = USART_StopBits_1; 
	USART_InitStructure.USART_Parity = USART_Parity_No; 
	USART_InitStructure.USART_HardwareFlowControl =USART_HardwareFlowControl_None; 
	USART_InitStructure.USART_Mode =USART_Mode_Rx|USART_Mode_Tx; //�շ�ģʽ
	USART_Init(USART1, &USART_InitStructure);

	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);//���������ȼ���λ1�������ȼ���λ��3	
	NVIC_InitStructure.NVIC_IRQChannel=USART1_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0x00;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority=0x00;
	NVIC_InitStructure.NVIC_IRQChannelCmd=ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
	USART_ITConfig(USART1,USART_IT_RXNE ,ENABLE);//�򿪽����ж�ʹ��
	USART_Cmd(USART1, ENABLE);//�򿪴���
	USART_ClearFlag(USART1,USART_FLAG_TC);
	
}


void USART1_IRQHandler(void)
{
	static u8 count =0;

	if(USART_GetITStatus(USART1,USART_IT_RXNE)==SET)
	{
		if(count<96)
		{
			while(USART_GetFlagStatus(USART1, USART_FLAG_RXNE)==RESET);//�ȴ��������	
			Tx_Buf[count++]=(USART_ReceiveData(USART1));	
      			
			if(count==96)
			{ 
				count=0;
				Tx_Flag_Over=1;  //һ֡���ݽ������
							
      }			
		}	
		USART_ClearITPendingBit(USART1,USART_IT_RXNE );//����жϱ�־
		
			
  }
}

