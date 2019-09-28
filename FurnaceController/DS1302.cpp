#include "DS1302.h"

DS1302::DS1302() {}

void DS1302::_DS1302_start(void)
{
	digitalWrite(DS1302_CE_PIN, LOW);
	pinMode(DS1302_CE_PIN, OUTPUT);
	digitalWrite(DS1302_SCLK_PIN, LOW);
	pinMode(DS1302_SCLK_PIN, OUTPUT);
	pinMode(DS1302_IO_PIN, OUTPUT);
	digitalWrite(DS1302_CE_PIN, HIGH);
	delayMicroseconds(4);
}

void DS1302::_DS1302_stop(void)
{
	digitalWrite(DS1302_CE_PIN, LOW);
	delayMicroseconds(4);
}

uint8_t _DS1302_toggleread(void)
{
	uint8_t i, data;
	data = 0;
	for(i = 0; i <= 7; i++)
	{
		digitalWrite(DS1302_SCLK_PIN, HIGH);
		delayMicroseconds(1);
		digitalWrite(DS1302_SCLK_PIN, LOW);
		delayMicroseconds(1);
		bitWrite(data, i, digitalRead(DS1302_IO_PIN));
	}

	return(data);
}

void DS1302::_DS1302_togglewrite(uint8_t data, uint8_t release)
{
	for(int i = 0; i <= 7; i++)
	{ 
		digitalWrite(DS1302_IO_PIN, bitRead(data, i));
		delayMicroseconds(1);
		digitalWrite(DS1302_SCLK_PIN, HIGH);
		delayMicroseconds(1);
		if(release && i == 7)
		{
			pinMode(DS1302_IO_PIN, INPUT);

			#if ARDUINO < 10003
				// For Arduino 1.0.3, removing the pull-up is no longer needed.
				// Setting the pin as 'INPUT' will already remove the pull-up.
				digitalWrite(DS1302_IO, LOW);
			#endif
		}
		else
		{
			digitalWrite(DS1302_SCLK_PIN, LOW);
			delayMicroseconds(1);
		}
	}
}

uint8_t DS1302::DS1302_read(int address)
{
	uint8_t data;
	bitSet(address, DS1302_READBIT);
	_DS1302_start();
	_DS1302_togglewrite(address, true);
	data = _DS1302_toggleread();
	_DS1302_stop();

	return(data);
}

void DS1302::DS1302_write(int address, uint8_t data)
{
	bitClear(address, DS1302_READBIT); 
	_DS1302_start();
	_DS1302_togglewrite(address, false); 
	_DS1302_togglewrite(data, false); 
	_DS1302_stop();
}

void DS1302::DS1302_clock_burst_read(uint8_t *p)
{
	_DS1302_start();
	_DS1302_togglewrite(DS1302_CLOCK_BURST_READ, true);
	for(int i = 0; i < 8; i++)
	{
		*p++ = _DS1302_toggleread();
	}
	_DS1302_stop();
}

void DS1302::DS1302_clock_burst_write(uint8_t *p)
{
	_DS1302_start();
	_DS1302_togglewrite(DS1302_CLOCK_BURST_WRITE, false);
	for(int i = 0; i < 8; i++)
	{
		_DS1302_togglewrite(*p++, false);
	}
	_DS1302_stop();
}

void DS1302::init(int seconds, int minutes, int hours, int dayOfMonth, int month, int year)
{
	DS1302_write(DS1302_ENABLE, 0);
	DS1302_write(DS1302_TRICKLE, 0x00);
	memset((char *) &dsDateTime, 0, sizeof(dsDateTime));
	dsDateTime.Seconds    = bin2bcd_l(seconds);
	dsDateTime.Seconds10  = bin2bcd_h(seconds);
	dsDateTime.CH         = 0;
	dsDateTime.Minutes    = bin2bcd_l(minutes);
	dsDateTime.Minutes10  = bin2bcd_h(minutes);
	dsDateTime.h24.Hour		= bin2bcd_l(hours);
	dsDateTime.h24.Hour10		= bin2bcd_h(hours);
	dsDateTime.Date       = bin2bcd_l(dayOfMonth);
	dsDateTime.Date10     = bin2bcd_h(dayOfMonth);
	dsDateTime.Month      = bin2bcd_l(month);
	dsDateTime.Month10    = bin2bcd_h(month);
	dsDateTime.Year       = bin2bcd_l(year - 2000);
	dsDateTime.Year10     = bin2bcd_h(year - 2000);
	dsDateTime.WP = 0;
	DS1302_clock_burst_write((uint8_t *) &dsDateTime);
}

void DS1302::read(void)
{
	DS1302_clock_burst_read((uint8_t *) &dsDateTime);
}

DateTime DS1302::Now(void)
{
	read();
	DateTime date;
	date.Year    = 2000 + bcd2bin(dsDateTime.Year10, dsDateTime.Year);
	date.Month   = bcd2bin(dsDateTime.Month10, dsDateTime.Month);
	date.Day     = bcd2bin(dsDateTime.Date10, dsDateTime.Date);
	date.Hours   = bcd2bin(dsDateTime.h24.Hour10, dsDateTime.h24.Hour);
	date.Minutes = bcd2bin(dsDateTime.Minutes10, dsDateTime.Minutes);
	date.Seconds = bcd2bin(dsDateTime.Seconds10, dsDateTime.Seconds);
	return date;
}
