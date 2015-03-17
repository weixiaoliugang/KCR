//第一组数据会少一到两帧，修改程序可第一桢去掉
#include <stm32f10x.h>
#include "NRF2401L.h"
#include "SYSTEM.h"

void EXIT_Disable(void)//关中断
{
	EXTI_InitTypeDef EXTI_InitStructure;//注意此结构体的中的内容
  
	//GPIOG.8 中断线以及中断初始化配置   下降沿触发
	GPIO_EXTILineConfig(GPIO_PortSourceGPIOG, GPIO_PinSource8);
	EXTI_InitStructure.EXTI_Line=EXTI_Line8;
  EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt; 
  EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling; //下降沿触发
  EXTI_InitStructure.EXTI_LineCmd =DISABLE;
	EXTI_Init(&EXTI_InitStructure);
}
void EXIT_Enable(void)//关中断
{
	EXTI_InitTypeDef EXTI_InitStructure;//注意此结构体的中的内容
  
	//GPIOG.8 中断线以及中断初始化配置   下降沿触发
	GPIO_EXTILineConfig(GPIO_PortSourceGPIOG, GPIO_PinSource8);
	EXTI_InitStructure.EXTI_Line=EXTI_Line8;
  EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt; 
  EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling; //下降沿触发
  EXTI_InitStructure.EXTI_LineCmd =ENABLE;
	EXTI_Init(&EXTI_InitStructure);
}


void EXIT_Init(void)
{
	EXTI_InitTypeDef EXTI_InitStructure;//注意此结构体的中的内容
	NVIC_InitTypeDef NVIC_InitStructure;
  
	
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_AFIO,ENABLE);//打开GPIO的外设时钟和IO口的复用时钟(用作EXIT外部中断，要开启AFIO,使用默认的复用功能，就不必开启AFIO)

	
	//GPIOG.8 中断线以及中断初始化配置   下降沿触发
	GPIO_EXTILineConfig(GPIO_PortSourceGPIOG, GPIO_PinSource8);
	EXTI_InitStructure.EXTI_Line=EXTI_Line8;
  EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt; 
  EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling; //下降沿触发
  EXTI_InitStructure.EXTI_LineCmd =ENABLE;
	EXTI_Init(&EXTI_InitStructure);
	
  NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);//设置抢占优先级为1位，响应优先级为3位
	NVIC_InitStructure.NVIC_IRQChannel=EXTI9_5_IRQn ;//配置中断向量
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0X01;//抢占优先级为0
	NVIC_InitStructure.NVIC_IRQChannelSubPriority=0X00;//响应优先级为0
	NVIC_InitStructure.NVIC_IRQChannelCmd=ENABLE;//中断使能
	NVIC_Init(&NVIC_InitStructure);
}

/*****中断函数部分*****************************************************/
/*当用到EXTI9_5_IRQHandler和EXTI15_9_IRQHandler时要首先判断具体的中断线（用EXTI_GetITStatus函数）*/

void EXTI9_5_IRQHandler(void)
{	
	
	u8 Respond[32];//回执信号
	u8 count;
	
	if(EXTI_GetITStatus(EXTI_Line8)==SET)
	{
		NRF24L01_RxPacket(Respond);
		for(count=0;count<32;count++)
		{
			USART_SendData(USART1,Respond[count]);//向pc机发送回执信号
			while(USART_GetFlagStatus(USART1,USART_FLAG_TC)!=SET);//等待发送结束
		}	
  }	
	EXTI_ClearITPendingBit(EXTI_Line8);//清除中断标志位，避免多次进入中断	
	
	
	
}

