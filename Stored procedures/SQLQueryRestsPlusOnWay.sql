--select * from PartnerPoints
--SELECT DATEDIFF(day, GETDATE(), );

use YstTerminal
DECLARE @MaxDaysFromPortToStock int =7
DECLARE @MaxDaysFromProductionToStock int =45

--  0 - rests, 1- onway , 2-mixed
DECLARE @TypeOfRests int=2


DECLARE @PartnerPointId int=134
DECLARE @DaysToMainDepartment int
DECLARE @ProductOnRests TABLE
(
 ProductId INT,  
 Rest INT,  
 DepartmentId INT,  
 DaysToDepartment INT 
)

DECLARE @ProductOnWay TABLE
(
 ProductId INT,  
 RestOnWay INT,  
 DaysToDepartmentOnWay INT
)

SELECT @DaysToMainDepartment = DaysToMainDepartment FROM PartnerPoints Where PartnerPointId=@PartnerPointId 

IF OBJECT_ID('tempdb..#RestsOptimal') IS NOT NULL DROP TABLE #RestsOptimal
  CREATE TABLE #RestsOptimal
(   ProductId INT,  Rest INT,  DepartmentId INT,  DaysToDepartment Int) ;


/*
* Get optimal rests from 2 stocks
*/
if (@TypeOfRests=0 OR @TypeOfRests=2)
BEGIN
   WITH CTE_RestsAndDays
  AS
  (SELECT RestOfProducts.ProductId ProductId, RestOfProducts.Rest, RestOfProducts.DepartmentId DepartmentId, DepartmentsAvailable.DaysToDepartment
FROM RestOfProducts
INNER JOIN
(
SELECT 5 DepartmentID, DaysToMainDepartment DaysToDepartment FROM PartnerPoints Where PartnerPointId=@PartnerPointId
UNION ALL
SELECT DepartmentID,DaysToDepartment FROM PartnerPoints Where PartnerPointId=@PartnerPointId AND IsNull(DepartmentID,-1)<>-1
) DepartmentsAvailable
ON RestOfProducts.DepartmentId=DepartmentsAvailable.DepartmentId )
INSERT INTO @ProductOnRests
SELECT CTE_RestsAndDays.ProductId, Rest,  DepartmentId, CTE_RestsAndDays.DaysToDepartment FROM  CTE_RestsAndDays
INNER JOIN
( SELECT ProductId, MIN(DaysToDepartment) DaysToDepartment FROM CTE_RestsAndDays
GROUP BY ProductId ) RestsWithMinDelivery
ON CTE_RestsAndDays.ProductId=RestsWithMinDelivery.ProductId 
AND CTE_RestsAndDays.DaysToDepartment=RestsWithMinDelivery.DaysToDepartment 
END
/*
* Get optimal products from way
*/
if @TypeOfRests>0 
begin
INSERT INTO @ProductOnWay
SELECT ProductId,RestOnWay ,DATEDIFF(day, GETDATE(),DateToDepartment) DaysToDepartmentOnWay FROM
(
SELECT ProductId, SUM(Count) RestOnWay, 0 DepartmentId, DATEADD(DAY, @DaysToMainDepartment, MAX(IIF(DateOfArrivalToStock<GETDATE(),GETDATE(),DateOfArrivalToStock))) DateToDepartment  FROM 
(
SELECT ProductId,  Count, /*ProdOrWay,DateOfArrival,*/ DATEDIFF(day, GETDATE(),DateOfArrival ) DateOfArrival,IIF(ProdOrWay=0,DATEADD(DAY, @MaxDaysFromProductionToStock, DateOfArrival),DATEADD(DAY, @MaxDaysFromPortToStock, DateOfArrival)) DateOfArrivalToStock 
from OnWayItems ) B
GROUP BY ProductId 
)ProductsOnWay
end

INSERT INTO #RestsOptimal 
SELECT ProductId, IIF(DepartmentId=0, RestOnWay,Rest) Rest, IIF(DepartmentId=0,DaysToDepartmentOnWay, DaysToDepartment) DaysToDepartment, DepartmentId
FROM 
(
SELECT ProductId, Rest, RestOnWay,DaysToDepartment,DaysToDepartmentOnWay,
IIF(Rest>=12,DepartmentId,IIF(Rest >= RestOnWay,DepartmentId,0)) DepartmentId
FROM (
SELECT ProductId , SUM(Rest) Rest, Sum (RestOnWay) RestOnWay, MAX(DepartmentId) DepartmentId, MAX(DaysToDepartment) DaysToDepartment, MAX(DaysToDepartmentOnWay) DaysToDepartmentOnWay

 FROM
(SELECT ProductId,Rest Rest, 0 RestOnWay,DepartmentId,DaysToDepartment, 0 DaysToDepartmentOnWay FROM @ProductOnRests
UNION ALL
SELECT ProductId,0 Rest, RestOnWay,0 DepartmentId,0 DaysToDepartment,DaysToDepartmentOnWay FROM @ProductOnWay ) A
GROUP BY ProductId )B
) D 
SELECT * FROM #RestsOptimal
--WHERE DepartmentId=0
