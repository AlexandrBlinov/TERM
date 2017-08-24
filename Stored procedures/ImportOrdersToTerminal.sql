
DECLARE @xmlData  as XML
DECLARE @Message nvarchar(max) = '' 

Declare @sqlBulkImportText as varchar(max)
DECLARE @results table (result xml)
DECLARE @ErrorNumber int = 0

DECLARE @OrderDetailsToUpdate table (OrderId INT )

DECLARE @FilePath  nvarchar(max)

SET @FilePath = 'c:\ImportData\Orders.xml'


/*  bulk  import from  file */
 SET @sqlBulkImportText= '( SELECT * FROM  OPENROWSET(BULK '+''''+ @FilePath+''''+', SINGLE_CLOB) A )'

INSERT INTO @results EXEC (@sqlBulkImportText)

SELECT @xmlData = result FROM @results;


  
IF OBJECT_ID('tempdb..#OrdersToImport') IS NOT NULL DROP TABLE #OrdersToImport
IF OBJECT_ID('tempdb..#OrdersDetailsToImport') IS NOT NULL DROP TABLE #OrdersDetailsToImport
IF OBJECT_ID('tempdb..#OrdersDetailsFull') IS NOT NULL DROP TABLE #OrdersDetailsFull

CREATE TABLE #OrdersToImport 
( OrderDate DATE, DeliveryDate DATE , PartnerId NVARCHAR(7) ,  OrderStatus INT,  DepartmentId INT,
  Total Decimal(15,2),  NumberIn1S NVARCHAR(8),  Order_guid UNIQUEIDENTIFIER,  Comments NVARCHAR(MAX),  IsReserve BIT) 
   CREATE TABLE #OrdersDetailsToImport 
( RowNumber INT, ProductId INT, Count INT, Price DECIMAL(10,2), Order_guid UNIQUEIDENTIFIER) 
   CREATE TABLE #OrdersDetailsFull 
( Order_guid UNIQUEIDENTIFIER, RowNumber INT, ProductId INT, Count INT, Price DECIMAL(10,2), PriceOfPoint DECIMAL(10,2),PriceOfClient DECIMAL(10,2) ) 


INSERT INTO  #OrdersToImport
 SELECT 
  ref.value('@OrderDate', 'DATE') OrderDate
 --,ref.value('@DeliveryDate', 'Date') DeliveryDate 
 ,CAST (nullif(ref.value('@DeliveryDate', 'Date'), '') as Date) DeliveryDate
,ref.value('@PartnerId', 'NVARCHAR(7)') PartnerId 
,ref.value('@OrderStatus', 'INT') OrderStatus 

,ref.value('@DepartmentId', 'INT') DepartmentId 
,ref.value('@Total', 'Decimal(15,2)') Total 
,ref.value('@NumberIn1S', 'NVARCHAR (8)') NumberIn1S 
 ,ref.value('@GUID', 'UNIQUEIDENTIFIER') Order_guid
 ,ref.value('@Comments', 'NVARCHAR (MAX)') Comments
 ,CAST(ISNULL (ref.value('@IsReserve', 'BIT'),0) AS BIT) IsReserve    
FROM @xmlData.nodes('/Orders/Order') xmlData(ref)   

MERGE INTO [Orders] AS Target
USING #OrdersToImport AS Source
ON Target.Order_guid=Source.Order_guid
WHEN MATCHED AND (
(IsNull(Target.PartnerId,0)<>IsNull(Source.PartnerId,0) )
OR (IsNull(Target.OrderStatus,0)<>IsNull(Source.OrderStatus,0))
OR (IsNull(Target.NumberIn1S,'')<>IsNull(Source.NumberIn1S,''))
OR (IsNull(Target.Total,0)<>IsNull(Source.Total,0))
OR (Target.IsReserve<>Source.IsReserve)
OR (IsNull(Target.DeliveryDate, CAST (0 AS Datetime))<>IsNull(Source.DeliveryDate,CAST (0 AS Datetime))) 
) THEN
UPDATE SET Target.PartnerId=Source.PartnerId, Target.OrderStatus=Source.OrderStatus,  Target.NumberIn1S=Source.NumberIn1S , Target.Total=Source.Total, Target.Comments=Source.Comments,
Target.IsReserve=Source.IsReserve, Target.DeliveryDate=Source.DeliveryDate;
--WHEN NOT MATCHED THEN
--INSERT (Order_guid,OrderDate,DeliveryDate,PartnerId,OrderStatus,DepartmentId, Total,NumberIn1S, Comments, IsReserve) 
--VALUES (Source.Order_guid,Source.OrderDate,Source.DeliveryDate, Source.PartnerId, Source.OrderStatus,Source.DepartmentId,Source.Total, Source.NumberIn1S, Source.Comments, Source.IsReserve); 




INSERT INTO  #OrdersDetailsToImport (RowNumber, ProductId, Count, Price, Order_guid)
SELECT OrderDetails.RowNumber, OrderDetails.ProductId, OrderDetails.Count, OrderDetails.Price,OrderDetails.Order_guid
  FROM 
( SELECT 
  ref.value('@ProductId', 'INT') ProductId,
  ref.value('@Count', 'INT') Count, 
 ref.value('@Price', 'DECIMAL(10,2)') Price, 
 ref.value('@RowNumber', 'INT') RowNumber, 
 ref.value('../../@GUID', 'UNIQUEIDENTIFIER') Order_guid
 
FROM @xmlData.nodes('/Orders/Order/OrderDetails/OrderDetail') xmlData(ref) ) OrderDetails
WHERE OrderDetails.Order_guid IN ( SELECT Order_guid  FROM Orders)




INSERT INTO #OrdersDetailsFull (Order_guid,RowNumber,ProductId,Count,Price,PriceOfPoint,PriceOfClient)
SELECT A.Order_guid Order_guid,A.RowNumber RowNumber  , 
A.ProductId ProductId, A.Count, A.Price, IsNull(OrderDetails.PriceOfPoint,0) PriceOfPoint, IsNull(OrderDetails.PriceOfClient,0) PriceOfClient FROM #OrdersDetailsToImport A
LEFT JOIN  OrderDetails ON
(A.Order_guid=OrderDetails.Order_guid AND A.ProductId=OrderDetails.ProductId)

DELETE FROM OrderDetails WHERE Order_guid IN (SELECT Order_guid FROM #OrdersDetailsToImport)

INSERT INTO OrderDetails (Order_guid,RowNumber,ProductId,Count,Price,PriceOfPoint,PriceOfClient)
 SELECT Order_guid,RowNumber,ProductId,Count,Price,PriceOfPoint,PriceOfClient FROM #OrdersDetailsFULL 

-- Проверка таб части
--SELECT * FROM OrderDetails WHERE Order_guid IN (SELECT Order_guid FROM #OrdersDetailsToImport)
--AND Order_guid='15403901-b1e6-11e4-a05e-d4ae52b5e909'
--ORDER BY Order_guid

--Проверка заказа
--SELECT * FROM Orders WHERE Order_guid ='15403901-b1e6-11e4-a05e-d4ae52b5e909'

