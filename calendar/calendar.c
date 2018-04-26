#include <stdio.h>

int main()
{
    int month, date, day = 1, last_month_it, last_weeks, last_days, next_days, next_day;
    char* month_names[] = { "January", "February", "March", "April", "May", "June", 
                            "July", "August", "September", "October", "November", "December" };
    int days[] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    for (month = 1; month <= 12; ++month)
    {
        puts(month_names[month - 1]);

        for (date = 1; date <= days[month - 1]; ++date) 
        {
            if (date == 1)
            {
                if (month == 1) 
                {
                    last_weeks = 31 - (day - 1);
                    last_days = 31;
                }
                else
                {
                    last_weeks = days[month - 2] - (day - 1);
                    last_days = days[month - 2];
                }
                
                for (last_month_it = last_weeks; last_month_it <= last_days; ++last_month_it)
                {
                    printf("%4d", last_month_it);
                }
            }

            printf("%4d", date);
            
            if (day == 6) 
            {
                puts("");
                day = 0;
                continue;
            }

            day++;
        }

        next_day = day;
        for (next_days = 1; next_day <= 6; ++next_days)
        {
            printf("%4d", next_days);
            next_day++;
        }

        puts("\n");
    }

    return 0;
}
