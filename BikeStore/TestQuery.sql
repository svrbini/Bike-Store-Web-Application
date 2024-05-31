use BikeStores

select * from products

select p.product_id, product_name, model_year, list_price, category_name, brand_name
from products as p
inner join categories on p.category_id=categories.category_id
inner join stocks on p.product_id=stocks.product_id
inner join brands on p.brand_id=brands.brand_id

select p.product_id, product_name, model_year, list_price, category_name, brand_name, SUM(quantity) as qnt
from products as p
inner join categories on p.category_id=categories.category_id
inner join stocks on p.product_id=stocks.product_id
inner join brands on p.brand_id=brands.brand_id
group by p.product_id,product_name, model_year, list_price, category_name, brand_name


select * from stocks
order by quantity desc

select product_id, SUM(quantity) as quantTot from stocks
group by product_id
order by quantTot

select * from stores
select city, state from stores

select * from orders

select * from categories

select * from brands

--store con più stock
select store_id, quantity
from stocks
where product_id = 6 AND quantity =(
	select MAX(quantity)
	from stocks
	where product_id = 6
)


INSERT INTO orders(customer_id, order_status, order_date, required_date, shipped_date, store_id,staff_id) 
VALUES(222,3,'20200101','20200103','20200103',2,3);

select * from orders
where order_id > 1500
select SCOPE_IDENTITY()
order by order_date

INSERT INTO order_items(order_id, item_id, product_id, quantity, list_price,discount) 
VALUES(1,1,20,1,599.99,0.2);




INSERT INTO order_items(column1, column2, column3, ...)
VALUES (value1, value2, value3, ...);

select * from stocks
where quantity > 0

SELECT * from stocks where product_id = 3
