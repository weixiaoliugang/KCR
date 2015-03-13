//第一组数据会少一到两帧，修改程序可第一桢去掉
#include <stm32f10x.h>
#include "NRF2401L.h"

extern u8 Rx_Buf[96];
extern u8 Rx_Flag_Over;

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
  EXTI_InitStructure.EXTI_LineCmd = ENABLE; 
  EXTI_Init(&EXTI_InitStructure);
	
  NVIC_PriorityGroupConfig(NVIC_PriorityGroup_0);//设置抢占优先级为0位，响应优先级为4位
	
	NVIC_InitStructure.NVIC_IRQChannel=EXTI9_5_IRQn ;//配置中断向量
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0X00;//抢占优先级为0
	NVIC_InitStructure.NVIC_IRQChannelSubPriority=0X00;//响应优先级为0
	NVIC_InitStructure.NVIC_IRQChannelCmd=ENABLE;//中断使能
	NVIC_Init(&NVIC_InitStructure);
}

/*****中断函数部分*****************************************************/
/*当用到EXTI9_5_IRQHandler和EXTI15_9_IRQHandler时要首先判断具体的中断线（用EXTI_GetITStatus函数）*/

void EXTI9_5_IRQHandler(void)
{	
	static u8 count=0;
	if(EXTI_GetITStatus(EXTI_Line8)==SET)
	{
		switch(count)                       //三次接受组成一帧
		{
			case 0:NRF24L01_RxPacket(Rx_Buf);count++;break;
			case 1:NRF24L01_RxPacket(Rx_Buf+32);count++;break;
			case 2:NRF24L01_RxPacket(Rx_Buf+64);count=0;Rx_Flag_Over=1;break;		
    }  		
		
		EXTI_ClearITPendingBit(EXTI_Line8);//清除中断标志位，避免多次进入中断	
	}
}

