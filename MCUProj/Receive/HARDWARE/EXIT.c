//��һ�����ݻ���һ����֡���޸ĳ���ɵ�һ��ȥ��
#include <stm32f10x.h>
#include "NRF2401L.h"
#include "SYSTEM.h"
#include "string.h"

extern u8 Rx_Buf_Bak[96];
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
			case 0:
            {
                NRF24L01_RxPacket(Rx_Buf);
                if((Rx_Buf[0]==0xff)&&(Rx_Buf[1]==0xff))
                {   
                    memcpy(Rx_Buf_Bak,Rx_Buf,32);                    
                    count++;
                    num++;
                }
                else
                {
                    count=0;
                }
                break;
            }
            
			case 1:
            {
                NRF24L01_RxPacket(Rx_Buf+32);
                if((Rx_Buf[32]==0x00)&&(Rx_Buf[33]==0x00))
                {   
                    memcpy(Rx_Buf_Bak+32,Rx_Buf+32,32);                    
                    count++;
                    num++;
                    
                }
                else
                {
                    count=0;
                }
                break;
            }
            
			case 2:
            {
                 NRF24L01_RxPacket(Rx_Buf+64);
                 if((Rx_Buf[64]==0x00)&&(Rx_Buf[65]==0x00))
                 { 
                    memcpy(Rx_Buf_Bak+64,Rx_Buf+64,32);     
                    count=0;
                    num++;
                    Rx_Flag_Over=1;       
                 }
                 else
                 {
                    count=0;
                 }
                 break;
            }		
        }  		
		EXTI_ClearITPendingBit(EXTI_Line8);//����жϱ�־λ�������ν����ж�	
	}
}

