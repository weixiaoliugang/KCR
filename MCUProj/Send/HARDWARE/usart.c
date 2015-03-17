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
	
	GPIO_PinRemapConfig(GPIO_Remap_USART1, ENABLE);//打开复用功能
	
	USART_InitStructure.USART_BaudRate = BaudRate;//波特率为
	USART_InitStructure.USART_WordLength = USART_WordLength_8b; 
	USART_InitStructure.USART_StopBits = USART_StopBits_1; 
	USART_InitStructure.USART_Parity = USART_Parity_No; 
	USART_InitStructure.USART_HardwareFlowControl =USART_HardwareFlowControl_None; 
	USART_InitStructure.USART_Mode =USART_Mode_Rx|USART_Mode_Tx; //收发模式
	USART_Init(USART1, &USART_InitStructure);

	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);//配置主优先级的位1，优先级的位数3	
	NVIC_InitStructure.NVIC_IRQChannel=USART1_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0x00;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority=0x00;
	NVIC_InitStructure.NVIC_IRQChannelCmd=ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
	USART_ITConfig(USART1,USART_IT_RXNE ,ENABLE);//打开接受中断使能
	USART_Cmd(USART1, ENABLE);//打开串口
	USART_ClearFlag(USART1,USART_FLAG_TC);
	
}


void USART1_IRQHandler(void)
{
	static u8 count =0;

	if(USART_GetITStatus(USART1,USART_IT_RXNE)==SET)
	{
		if(count<96)
		{
			while(USART_GetFlagStatus(USART1, USART_FLAG_RXNE)==RESET);//等待接受完成	
			Tx_Buf[count++]=(USART_ReceiveData(USART1));	
      			
			if(count==96)
			{ 
				count=0;
				Tx_Flag_Over=1;  //一帧数据接收完毕
							
      }			
		}	
		USART_ClearITPendingBit(USART1,USART_IT_RXNE );//清除中断标志
		
			
  }
}

