#ifndef DS1302_H
#define DS1302_H

#include <stdint.h>
#include <Arduino.h>

#define DS1302_SCLK_PIN 42   // CLOCK : 'CLK'
#define DS1302_IO_PIN   43   // DATA :  'DAT'
#define DS1302_CE_PIN   44   // Reset : 'RST'

#define bcd2bin(h, l) (((h) * 10) + (l))
#define bin2bcd_h(x)  ((x) / 10)
#define bin2bcd_l(x)  ((x) % 10)

#define DS1302_SECONDS           0x80
#define DS1302_MINUTES           0x82
#define DS1302_HOURS             0x84
#define DS1302_DATE              0x86
#define DS1302_MONTH             0x88
#define DS1302_YEAR              0x8C
#define DS1302_ENABLE            0x8E
#define DS1302_TRICKLE           0x90
#define DS1302_CLOCK_BURST       0xBE
#define DS1302_CLOCK_BURST_WRITE 0xBE
#define DS1302_CLOCK_BURST_READ  0xBF
#define DS1302_RAMSTART          0xC0
#define DS1302_RAMEND            0xFC
#define DS1302_RAM_BURST         0xFE
#define DS1302_RAM_BURST_WRITE   0xFE
#define DS1302_RAM_BURST_READ    0xFF

#define DS1302_D0 0
#define DS1302_D1 1
#define DS1302_D2 2
#define DS1302_D3 3
#define DS1302_D4 4
#define DS1302_D5 5
#define DS1302_D6 6
#define DS1302_D7 7

#define DS1302_READBIT DS1302_D0
#define DS1302_RC      DS1302_D6
#define DS1302_CH      DS1302_D7
#define DS1302_AM_PM   DS1302_D5
#define DS1302_12_24   DS1302_D7
#define DS1302_WP      DS1302_D7
#define DS1302_ROUT0   DS1302_D0
#define DS1302_ROUT1   DS1302_D1
#define DS1302_DS0     DS1302_D2
#define DS1302_DS1     DS1302_D2
#define DS1302_TCS0    DS1302_D4
#define DS1302_TCS1    DS1302_D5
#define DS1302_TCS2    DS1302_D6
#define DS1302_TCS3    DS1302_D7

#define AM  0
#define PM  1


class DateTime
{
  public:
    int Year = 0;
    int Month = 0;
    int Day = 0;
    int Hours = 0;
    int Minutes = 0;
    int Seconds = 0;

    String ToFurnaceString(void)
    {
      return String(Year) + "-" + String(Month) + "-" + String(Day) + "-" + String(Hours) + "-" + String(Minutes) + "-" + String(Seconds);
    }

    long ToSeconds(void)
    {
      long Y =  long(Year    * 31556926);
      long M =  long(Month   * 2629744);
      long D =  long(Day     * 86400);
      long H =  long(Hours   * 3600);
      long E =  long(Minutes * 60);
      return long(Y + M + D + H + E + Seconds);
    }

};



class DS1302
{
  public:
    struct {
      uint8_t Seconds: 4;
      uint8_t Seconds10: 3;
      uint8_t CH: 1;
      uint8_t Minutes: 4;
      uint8_t Minutes10: 3;
      uint8_t reserved1: 1;

      union
      {
        struct
        {
          uint8_t Hour: 4;
          uint8_t Hour10: 2;
          uint8_t reserved2: 1;
          uint8_t hour_12_24: 1;
        } h24;

        struct
        {
          uint8_t Hour: 4;
          uint8_t Hour10: 1;
          uint8_t AM_PM: 1;
          uint8_t reserved2: 1;
          uint8_t hour_12_24: 1;
        } h12;
      };

      uint8_t Date: 4;
      uint8_t Date10: 2;
      uint8_t reserved3: 2;
      uint8_t Month: 4;
      uint8_t Month10: 1;
      uint8_t reserved4: 3;
      uint8_t Day: 3;
      uint8_t reserved5: 5;
      uint8_t Year: 4;
      uint8_t Year10: 4;
      uint8_t reserved6: 7;
      uint8_t WP: 1;
    } dsDateTime;

    DS1302();
    void _DS1302_start(void);
    void _DS1302_stop(void);
    void _DS1302_togglewrite(uint8_t data, uint8_t release);
    uint8_t DS1302_read(int address);
    void DS1302_write(int address, uint8_t data);
    void DS1302_clock_burst_read(uint8_t *p);
    void DS1302_clock_burst_write(uint8_t *p);

    void init(int seconds, int minutes, int hours, int dayOfMonth, int month, int year);
    void read(void);
    DateTime Now(void);
};
#endif
