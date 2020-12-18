## Video Game World Sticker Sheet Generator

Video Game world is a local business in Charlotte North Carolina.  I have been a happy customer shopping at their locations for quite a while and am lucky to call the staff my friends.

Often they must update the prices of their inventory to ensure accuracy to the current market value.  These prices are physically printed as stickers and placed on the games which are on display to the customer.

There is an eCommerce engine that manages the inventory and provides an export of the inventory for employees in a csv format with a column named quantity.  This column represents how many copies of a game the store has within thei inventory.

The task is to transpose how ever many copies for a given game into that amount of rows.  So if a game called "Pacman" shows quantity of 7, there should be 7 rows generated with identical data like the name Pacman.

This serves as a flat-file input for the Sticker printer machine which just iterates over the flat list and prints one at a time (7 times for our Pacman example).

I was told that transposing this inventory manually was a pain as it was error-prone and time consuming so I decided to write this software.

Of course, this entire application could most likely be converted into an excel formula, but the intention was to make this simple and user-friendly and to be honest I wanted to further my knowledged of WPF and XAML :)

### Target Audience
This software is intended to be used by Video Game World employees.

### Original Creation Date
8/12/2020

### Author
Evan Pittfield (aka Box-Guy) - evanpittfield@gmail.com
### Notes
- This app skips records that have total quantity of zero

- This app skips records that have total quantity of negative numbers

- This App outputs both a result csv and a log file in the location specified by the user.  
   The log file contains execution details and exception details
