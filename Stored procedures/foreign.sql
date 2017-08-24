DECLARE  @isforeign binary =0;
DECLARE  @ruRU nvarchar(5) ='ru-RU';
DECLARE  @PartnerId nvarchar(7) ='92523';
DECLARE @CustomDutyVal int=0;
DECLARE @VatVal int=0;

SELECT TOP 1 @isforeign=1, @CustomDutyVal = CustomDutyVal, @VatVal=VatVal FROM Partners WHERE PartnerId=@PartnerId AND (NOT Culture is null)  AND (Culture<>@ruRU)
print @VatVal
/*
IF EXISTS (SELECT TOP 1 1 FROM Partners WHERE PartnerId=@PartnerId AND (NOT Culture is null)  AND (Culture<>@ruRU))
SET @isforeign=1;
print @isforeign
*/
/*IF (@isforeign=1)
BEGIN
SELECT @CustomDutyVal = CustomDutyVal, @VatVal=VatVal FROM Partners WHERE PartnerId=@PartnerId AND (NOT Culture is null)  AND (Culture<>@ruRU)

END
PRINT @CustomDutyVal */

--SELECT * FROM Partners WHERE (NOT Culture is null)  AND (Culture<>@ruRU) AND CustomDutyVal>0