use YstDatabase
--select * from Sales where Docdate> '28.01.2016' 
update  sales set TotalSum=A.totalsum from sales
INNER JOIN 
(select GuidIn1S,sum(count*Price) totalsum from SaleDetails
group by GuidIn1S )A 
ON sales.GuidIn1S=A.GuidIn1S