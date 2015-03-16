//��һ�����ݻ���һ����֡���޸ĳ���ɵ�һ��ȥ��
#include <stm32f10x.h>
#include "NRF2401L.h"
#include "SYSTEM.h"

extern u8 Rx_Buf[96];
extern u8 Rx_Flag_Over;
extern u8 num;

void EXIT_Disable(void)//���ж�
{
	EXTI_InitTypeDef EXTI_InitStructure;//ע��˽ṹ����е�����
  
	//GPIOG.8 �ж����Լ��жϳ�ʼ������   �½��ش���
	GPIO_EXTILineConfig(GPIO_PortSourceGPIOG, GPIO_PinSource8);
	EXTI_InitStructure.EXTI_Line=EXTI_Line8;
  EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt; 
  EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling; //�½��ش���
  EXTI_InitStructure.EXTI_LineCmd =DISABLE;
	EXTI_Init(&EXTI_InitStructure);
	
 
}

void EXIT_Enable(void)//���ж�
{
	EXTI_InitTypeDef EXTI_InitStructure;//ע��˽ṹ����е�����
	 
	//GPIOG.8 �ж����Լ��жϳ�ʼ������   �½��ش���
	GPIO_EXTILineConfig(GPIO_PortSourceGPIOG, GPIO_PinSource8);
	EXTI_InitStructure.EXTI_Line=EXTI_Line8;
  EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt; 
  EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling; //�½��ش���
  EXTI_InitStructure.EXTI_LineCmd =ENABLE;
	EXTI_Init(&EXTI_InitStructure);
	
}


void EXIT_Init(void)
{
	EXTI_InitTypeDef EXTI_InitStructure;//ע��˽ṹ����е�����
	NVIC_InitTypeDef NVIC_InitStructure;
  
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_AFIO,ENABLE);//��GPIO������ʱ�Ӻ�IO�ڵĸ���ʱ��(����EXIT�ⲿ�жϣ�Ҫ����AFIO,ʹ��Ĭ�ϵĸ��ù��ܣ��Ͳ��ؿ���AFIO)

	
	//GPIOG.8 �ж����Լ��жϳ�ʼ������   �½��ش���
	GPIO_EXTILineConfig(GPIO_PortSourceGPIOG, GPIO_PinSource8);
	EXTI_InitStructure.EXTI_Line=EXTI_Line8;
  EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt; 
  EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling; //�½��ش���
  EXTI_InitStructure.EXTI_LineCmd =ENABLE;
	EXTI_Init(&EXTI_InitStructure);
	
  NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);//������ռ���ȼ�Ϊ1λ����Ӧ���ȼ�Ϊ3λ
	NVIC_InitStructure.NVIC_IRQChannel=EXTI9_5_IRQn ;//�����ж�����
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0X01;//��ռ���ȼ�Ϊ0
	NVIC_InitStructure.NVIC_IRQChannelSubPriority=0X00;//��Ӧ���ȼ�Ϊ0
	NVIC_InitStructure.NVIC_IRQChannelCmd=ENABLE;//�ж�ʹ��
	NVIC_Init(&NVIC_InitStructure);
}


/*****�жϺ�������*****************************************************/
/*���õ�EXTI9_5_IRQHandler��EXTI15_9_IRQHandlerʱҪ�����жϾ�����ж��ߣ���EXTI_GetITStatus������*/

void EXTI9_5_IRQHandler(void)
{	
	static u8 count=0;
	if(EXTI_GetITStatus(EXTI_Line8)==SET)
	{
		switch(count)                       //���ν������һ֡
		{
			case 0:NRF24L01_RxPacket(Rx_Buf);count++;num++;break;
			case 1:NRF24L01_RxPacket(Rx_Buf+32);count++;num++;break;
			case 2:NRF24L01_RxPacket(Rx_Buf+64);count=0;Rx_Flag_Over=1;num++;break;		
    }  		
		EXTI_ClearITPendingBit(EXTI_Line8);//����жϱ�־λ�������ν����ж�	
	}
}

