#include "NRF2401L.h"
#include "spi.h"

const u8 TX_ADDRESS[TX_ADR_WIDTH]={0x34,0x43,0x10,0x10,0x01}; //·¢ËÍµØÖ·
const u8 RX_ADDRESS[RX_ADR_WIDTH]={0x34,0x43,0x10,0x10,0x01}; //·¢ËÍµØÖ·


void NRF24L01_Init(void)
{ 	
	GPIO_InitTypeDef GPIO_InitStructure;
  SPI_InitTypeDef  SPI_InitStructure;

	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB|RCC_APB2Periph_GPIOG, ENABLE);	 //Ê¹ÄÜPB,D,G¶Ë¿ÚÊ±ÖÓ
  	
			
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_12;				 //PB12ÉÏÀ­ ·ÀÖ¹W25XµÄ¸ÉÈÅ(ºÜÖØÒª£¬Î»ÖÃÒ»¶¨¾ÍÔÚ´Ë£©
 	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP; 		 //ÍÆÍìÊä³ö
 	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
 	GPIO_Init(GPIOB, &GPIO_InitStructure);	//³õÊ¼»¯Ö¸¶¨IO
 	GPIO_SetBits(GPIOB,GPIO_Pin_12);//ÉÏÀ­
	
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_6|GPIO_Pin_7;	//PG6 7 ÍÆÍì 	  
 	GPIO_Init(GPIOG, &GPIO_InitStructure);//³õÊ¼»¯Ö¸¶¨IO
  
	GPIO_InitStructure.GPIO_Pin  = GPIO_Pin_8;  //	GPIOG.8->IRQ 
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING; //PG8 ¸¡¿ÕÊäÈë 
	GPIO_Init(GPIOG, &GPIO_InitStructure);
	
	
	GPIO_ResetBits(GPIOG,GPIO_Pin_6|GPIO_Pin_7|GPIO_Pin_8);//PG6,7,8ÉÏÀ­	
	
  SPI2_Init();    		//³õÊ¼»¯SPI	 
 
	SPI_Cmd(SPI2, DISABLE); // SPIÍâÉè²»Ê¹ÄÜ

	SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex;  //SPIÉèÖÃÎªË«ÏßË«ÏòÈ«Ë«¹¤
	SPI_InitStructure.SPI_Mode = SPI_Mode_Master;		//SPIÖ÷»ú
  SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;		//·¢ËÍ½ÓÊÕ8Î»Ö¡½á¹¹
	SPI_InitStructure.SPI_CPOL = SPI_CPOL_Low;		//Ê±ÖÓĞü¿ÕµÍ
	SPI_InitStructure.SPI_CPHA = SPI_CPHA_1Edge;	//Êı¾İ²¶»ñÓÚµÚ1¸öÊ±ÖÓÑØ
	SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;		//NSSĞÅºÅÓÉÈí¼ş¿ØÖÆ
	SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_16;		//¶¨Òå²¨ÌØÂÊÔ¤·ÖÆµµÄÖµ:²¨ÌØÂÊÔ¤·ÖÆµÖµÎª16
	SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;	//Êı¾İ´«Êä´ÓMSBÎ»¿ªÊ¼
	SPI_InitStructure.SPI_CRCPolynomial = 7;	//CRCÖµ¼ÆËãµÄ¶àÏîÊ½
	SPI_Init(SPI2, &SPI_InitStructure);  //¸ù¾İSPI_InitStructÖĞÖ¸¶¨µÄ²ÎÊı³õÊ¼»¯ÍâÉèSPIx¼Ä´æÆ÷
 
	SPI_Cmd(SPI2, ENABLE); //Ê¹ÄÜSPIÍâÉè
			 
	NRF24L01_CE_Low ; 			//Ê¹ÄÜ24L01
	NRF24L01_CSN_High;			//SPIÆ¬Ñ¡È¡Ïû 
 
	 		 	 
}
//¼ì²â24L01ÊÇ·ñ´æÔÚ
//·µ»ØÖµ:0£¬³É¹¦;1£¬Ê§°Ü	
u8 NRF24L01_Check(void)
{
	u8 buf[5]={0XA5,0XA5,0XA5,0XA5,0XA5};
	u8 i;
	SPI2_SetSpeed(SPI_BaudRatePrescaler_4); //spiËÙ¶ÈÎª9Mhz£¨24L01µÄ×î´óSPIÊ±ÖÓÎª10Mhz£©   	 
	NRF24L01_Write_Buf(WRITE_REG_NRF+TX_ADDR,buf,5);//Ğ´Èë5¸ö×Ö½ÚµÄµØÖ·.	
	NRF24L01_Read_Buf(TX_ADDR,buf,5); //¶Á³öĞ´ÈëµÄµØÖ·  
	for(i=0;i<5;i++)if(buf[i]!=0XA5)break;	 							   
	if(i!=5)return 1;//¼ì²â24L01´íÎó	
	return 0;		 //¼ì²âµ½24L01
}	 

//SPIĞ´¼Ä´æÆ÷
//reg:Ö¸¶¨¼Ä´æÆ÷µØÖ·
//value:Ğ´ÈëµÄÖµ
u8 NRF24L01_Write_Reg(u8 reg,u8 value)
{
	  u8 status;	
   	NRF24L01_CSN_Low;                 //Ê¹ÄÜSPI´«Êä
  	status =SPI2_ReadWriteByte(reg);//·¢ËÍ¼Ä´æÆ÷ºÅ 
  	SPI2_ReadWriteByte(value);      //Ğ´Èë¼Ä´æÆ÷µÄÖµ
  	NRF24L01_CSN_High;                 //½ûÖ¹SPI´«Êä	   
  	return(status);       			//·µ»Ø×´Ì¬Öµ
}

//¶ÁÈ¡SPI¼Ä´æÆ÷Öµ
//reg:Òª¶ÁµÄ¼Ä´æÆ÷
u8 NRF24L01_Read_Reg(u8 reg)
{
		u8 reg_val;	    
		NRF24L01_CSN_Low;          //Ê¹ÄÜSPI´«Êä		
		SPI2_ReadWriteByte(reg);   //·¢ËÍ¼Ä´æÆ÷ºÅ
		reg_val=SPI2_ReadWriteByte(0XFF);//¶ÁÈ¡¼Ä´æÆ÷ÄÚÈİ
		NRF24L01_CSN_High;          //½ûÖ¹SPI´«Êä		    
		return(reg_val);           //·µ»Ø×´Ì¬Öµ
}	

//ÔÚÖ¸¶¨Î»ÖÃ¶Á³öÖ¸¶¨³¤¶ÈµÄÊı¾İ
//reg:¼Ä´æÆ÷(Î»ÖÃ)
//*pBuf:Êı¾İÖ¸Õë
//len:Êı¾İ³¤¶È
//·µ»ØÖµ,´Ë´Î¶Áµ½µÄ×´Ì¬¼Ä´æÆ÷Öµ 
u8 NRF24L01_Read_Buf(u8 reg,u8 *pBuf,u8 len)
{
	  u8 status,u8_ctr;	       
  	NRF24L01_CSN_Low;           //Ê¹ÄÜSPI´«Êä
  	status=SPI2_ReadWriteByte(reg);//·¢ËÍ¼Ä´æÆ÷Öµ(Î»ÖÃ),²¢¶ÁÈ¡×´Ì¬Öµ   	   
 	  for(u8_ctr=0;u8_ctr<len;u8_ctr++)pBuf[u8_ctr]=SPI2_ReadWriteByte(0XFF);//¶Á³öÊı¾İ
  	NRF24L01_CSN_High;       //¹Ø±ÕSPI´«Êä
  	return status;        //·µ»Ø¶Áµ½µÄ×´Ì¬Öµ
}

//ÔÚÖ¸¶¨Î»ÖÃĞ´Ö¸¶¨³¤¶ÈµÄÊı¾İ
//reg:¼Ä´æÆ÷(Î»ÖÃ)
//*pBuf:Êı¾İÖ¸Õë
//len:Êı¾İ³¤¶È
//·µ»ØÖµ,´Ë´Î¶Áµ½µÄ×´Ì¬¼Ä´æÆ÷Öµ
u8 NRF24L01_Write_Buf(u8 reg, u8 *pBuf, u8 len)
{
		u8 status,u8_ctr;	    
		NRF24L01_CSN_Low;          //Ê¹ÄÜSPI´«Êä
		status = SPI2_ReadWriteByte(reg);//·¢ËÍ¼Ä´æÆ÷Öµ(Î»ÖÃ),²¢¶ÁÈ¡×´Ì¬Öµ
		for(u8_ctr=0; u8_ctr<len; u8_ctr++)SPI2_ReadWriteByte(*pBuf++); //Ğ´ÈëÊı¾İ	 
		NRF24L01_CSN_High;       //¹Ø±ÕSPI´«Êä
		return status;          //·µ»Ø¶Áµ½µÄ×´Ì¬Öµ
}	

//Æô¶¯NRF24L01·¢ËÍÒ»´ÎÊı¾İ
//txbuf:´ı·¢ËÍÊı¾İÊ×µØÖ·
//·µ»ØÖµ:·¢ËÍÍê³É×´¿ö
u8 NRF24L01_TxPacket(u8 *txbuf)
{
	u8 sta;
 	SPI2_SetSpeed(SPI_BaudRatePrescaler_8);//spiËÙ¶ÈÎª9Mhz£¨24L01µÄ×î´óSPIÊ±ÖÓÎª10Mhz£©   
	NRF24L01_CE_Low;
  NRF24L01_Write_Buf(WR_TX_PLOAD,txbuf,TX_PLOAD_WIDTH);//Ğ´Êı¾İµ½TX BUF  32¸ö×Ö½Ú
 	NRF24L01_CE_High;//Æô¶¯·¢ËÍ	   
	while(NRF24L01_IRQ!=0);//µÈ´ı·¢ËÍÍê³É
	sta=NRF24L01_Read_Reg(STATUS);  //¶ÁÈ¡×´Ì¬¼Ä´æÆ÷µÄÖµ 
	sta&=0xf1;//(ºÜÖØÒª£¬Ä¿µÄ£ºÓĞ0x00±äÎª0x0e)
	NRF24L01_Write_Reg(WRITE_REG_NRF+STATUS,sta);//Çå³ıTX_DS»òMAX_RTÖĞ¶Ï±êÖ¾
	if(sta&MAX_TX)//´ïµ½×î´óÖØ·¢´ÎÊı
	{
		NRF24L01_Write_Reg(FLUSH_TX,0xff);//Çå³ıTX FIFO¼Ä´æÆ÷ 
		return MAX_TX; 
	}
	if(sta&TX_OK)//·¢ËÍÍê³É
	{
		return TX_OK;
	}
	return 0xff;//ÆäËûÔ­Òò·¢ËÍÊ§°Ü
}

//Æô¶¯NRF24L01·¢ËÍÒ»´ÎÊı¾İ
//txbuf:´ı·¢ËÍÊı¾İÊ×µØÖ·
//·µ»ØÖµ:0£¬½ÓÊÕÍê³É£»ÆäËû£¬´íÎó´úÂë
u8 NRF24L01_RxPacket(u8 *rxbuf)
{
	
	u8 sta;
	SPI2_SetSpeed(SPI_BaudRatePrescaler_8); //spiËÙ¶ÈÎª9Mhz£¨24L01µÄ×î´óSPIÊ±ÖÓÎª10Mhz£©   
	sta=NRF24L01_Read_Reg(STATUS);  //¶ÁÈ¡×´Ì¬¼Ä´æÆ÷µÄÖµ 
	NRF24L01_Write_Reg(WRITE_REG_NRF+STATUS,(sta&0xf1));//Çå³ıTX_DS»òMAX_RTÖĞ¶Ï±êÖ¾(ºÜÖØÒª£¬Ä¿µÄ£ºÓĞ0x00±äÎª0x0e)
	if(sta&RX_OK)//½ÓÊÕµ½Êı¾İ
	{		
		NRF24L01_Read_Buf(RD_RX_PLOAD,rxbuf,RX_PLOAD_WIDTH);//¶ÁÈ¡Êı¾
		NRF24L01_Write_Reg(FLUSH_RX,0xff);//Çå³ıRX FIFO¼Ä´æÆ÷ 
		sta=NRF24L01_Read_Reg(STATUS);  //¶ÁÈ¡×´Ì¬¼Ä´æÆ÷µÄÖµ 
	  sta&=0xf1;//(ºÜÖØÒª£¬Ä¿µÄ£ºÓĞ0x00±äÎª0x0e)
	  NRF24L01_Write_Reg(WRITE_REG_NRF+STATUS,sta);//Çå³ıTX_DS»òMAX_RTÖĞ¶Ï±êÖ¾	
		
	}	   
	return 1;//Ã»ÊÕµ½ÈÎºÎÊı¾İ
}		

//¸Ãº¯Êı³õÊ¼»¯NRF24L01µ½RXÄ£Ê½
//ÉèÖÃRXµØÖ·,Ğ´RXÊı¾İ¿í¶È,Ñ¡ÔñRFÆµµÀ,²¨ÌØÂÊºÍLNA HCURR
//µ±CE±ä¸ßºó,¼´½øÈëRXÄ£Ê½,²¢¿ÉÒÔ½ÓÊÕÊı¾İÁË		   
void NRF24L01_RX_Mode(void)
{
	  u8 sta;
	  NRF24L01_CE_Low;
    NRF24L01_Write_Reg(WRITE_REG_NRF+CONFIG, 0x00);//	µôµç±£»¤
    sta=NRF24L01_Read_Reg(STATUS);  //¶ÁÈ¡×´Ì¬¼Ä´æÆ÷µÄÖµ 
	  sta&=0xf1;//(ºÜÖØÒª£¬Ä¿µÄ£ºÓĞ0x00±äÎª0x0e)
	  NRF24L01_Write_Reg(WRITE_REG_NRF+STATUS,sta);//Çå³ıTX_DS»òMAX_RTÖĞ¶Ï±êÖ(ºÜÖØÒª£¬Ä¿µÄ£ºÓĞ0x00±äÎª0x0e)¾
	  NRF24L01_Write_Reg(FLUSH_RX,0xff);//Çå³ıRX FIFO¼Ä´æÆ÷ 
	
  	NRF24L01_Write_Buf(WRITE_REG_NRF+RX_ADDR_P0,(u8*)RX_ADDRESS,RX_ADR_WIDTH);//Ğ´RX½ÚµãµØÖ·	  
  	NRF24L01_Write_Reg(WRITE_REG_NRF+EN_AA,0x01);    //Ê¹ÄÜÍ¨µÀ0µÄ×Ô¶¯Ó¦´ğ    
  	NRF24L01_Write_Reg(WRITE_REG_NRF+EN_RXADDR,0x01);//Ê¹ÄÜÍ¨µÀ0µÄ½ÓÊÕµØÖ·  	 
  	NRF24L01_Write_Reg(WRITE_REG_NRF+RF_CH,40);	     //ÉèÖÃRFÍ¨ĞÅÆµÂÊ		  
  	NRF24L01_Write_Reg(WRITE_REG_NRF+RX_PW_P0,RX_PLOAD_WIDTH);//Ñ¡ÔñÍ¨µÀ0µÄÓĞĞ§Êı¾İ¿í¶È 	    
  	NRF24L01_Write_Reg(WRITE_REG_NRF+RF_SETUP,0x0f);//ÉèÖÃTX·¢Éä²ÎÊı,0dbÔöÒæ,2Mbps,µÍÔëÉùÔöÒæ¿ªÆô 
  	NRF24L01_Write_Reg(WRITE_REG_NRF+CONFIG, 0x0f);//ÅäÖÃ»ù±¾¹¤×÷Ä£Ê½µÄ²ÎÊı;PWR_UP,EN_CRC,16BIT_CRC,½ÓÊÕÄ£Ê½£¬´ò¿ªËùÓĞÖĞ¶Ï
  	NRF24L01_CE_High; //CEÎª¸ß,½øÈë½ÓÊÕÄ£Ê½ 
}	

//¸Ãº¯Êı³õÊ¼»¯NRF24L01µ½TXÄ£Ê½
//ÉèÖÃTXµØÖ·,Ğ´TXÊı¾İ¿í¶È,ÉèÖÃRX×Ô¶¯Ó¦´ğµÄµØÖ·,Ìî³äTX·¢ËÍÊı¾İ,Ñ¡ÔñRFÆµµÀ,²¨ÌØÂÊºÍLNA HCURR
//PWR_UP,CRCÊ¹ÄÜ
//µ±CE±ä¸ßºó,¼´½øÈëRXÄ£Ê½,²¢¿ÉÒÔ½ÓÊÕÊı¾İÁË		   
//CEÎª¸ß´óÓÚ10us,ÔòÆô¶¯·¢ËÍ.	 
void NRF24L01_TX_Mode(void)
{	  
	  u8 sta;											 
	  NRF24L01_CE_Low;
	  NRF24L01_Write_Reg(WRITE_REG_NRF+CONFIG, 0x00);//	µôµç±£»¤
    sta=NRF24L01_Read_Reg(STATUS);  //¶ÁÈ¡×´Ì¬¼Ä´æÆ÷µÄÖµ 
	  sta&=0xf1;//(ºÜÖØÒª£¬Ä¿µÄ£ºÓĞ0x00±äÎª0x0e)
	  NRF24L01_Write_Reg(WRITE_REG_NRF+STATUS,sta);//Çå³ıTX_DS»òMAX_RTÖĞ¶Ï±êÖ¾
  	NRF24L01_Write_Buf(WRITE_REG_NRF+TX_ADDR,(u8*)TX_ADDRESS,TX_ADR_WIDTH);//Ğ´TX½ÚµãµØÖ· 
  	NRF24L01_Write_Buf(WRITE_REG_NRF+RX_ADDR_P0,(u8*)RX_ADDRESS,RX_ADR_WIDTH); //ÉèÖÃTX½ÚµãµØÖ·,Ö÷ÒªÎªÁËÊ¹ÄÜACK	  
  	NRF24L01_Write_Reg(WRITE_REG_NRF+EN_AA,0x01);     //Ê¹ÄÜÍ¨µÀ0µÄ×Ô¶¯Ó¦´ğ    
  	NRF24L01_Write_Reg(WRITE_REG_NRF+EN_RXADDR,0x01); //Ê¹ÄÜÍ¨µÀ0µÄ½ÓÊÕµØÖ·  
  	NRF24L01_Write_Reg(WRITE_REG_NRF+SETUP_RETR,0x1a);//ÉèÖÃ×Ô¶¯ÖØ·¢¼ä¸ôÊ±¼ä:500us + 86us;×î´ó×Ô¶¯ÖØ·¢´ÎÊı:10´Î
  	NRF24L01_Write_Reg(WRITE_REG_NRF+RF_CH,40);       //ÉèÖÃRFÍ¨µÀÎª40
  	NRF24L01_Write_Reg(WRITE_REG_NRF+RF_SETUP,0x0f);  //ÉèÖÃTX·¢Éä²ÎÊı,0dbÔöÒæ,2Mbps,µÍÔëÉùÔöÒæ¿ªÆô   
  	NRF24L01_Write_Reg(WRITE_REG_NRF+CONFIG,0x0e);    //ÅäÖÃ»ù±¾¹¤×÷Ä£Ê½µÄ²ÎÊı;PWR_UP,EN_CRC,16BIT_CRC,·¢ËÍÄ£Ê½£¬´ò¿ªËùÓĞÖĞ¶Ï
	  NRF24L01_CE_High;//CEÎª¸ß,10usºóÆô¶¯·¢ËÍ
}		   




