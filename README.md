# Checkout Kata

Implement a solution for a checkout kata that follows the requirements below.

- Given I have selected to add an item to the basket Then the item should be added to the basket
- Given items have been added to the basket Then the total cost of the basket should be calculated
- Given I have added a multiple of 3 lots of item `B` to the basket Then a promotion of `3 for 40` should be applied to every multiple of 3 (see: Grid 1).
- Given I have added a multiple of 2 lots of item `D` to the basket Then a promotion of `25% off` should be applied to every multiple of 2 (see: Grid 1).

_Grid 1:_
| Item SKU | Unit Price | Promotions |
| --- | --- | --- |
| A | 10 | |
| B | 15 | 3 for 40 |
| C | 40 | |
| D | 55 | 25% off for every 2 purchased together |

Items are priced individually. In addition, there are promotions which can affect the total price of the basket, for example, when 3 lots of item `D` are added to the basket then a 25% deduction is applied to the total cost of 2 of those items. The pricing changes frequently, so pricing should be independent of the checkout.
