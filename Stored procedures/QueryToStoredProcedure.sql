-- exec sp_executesql N'spGetDisks'
exec sp_executesql N'spGetDisks',N'@PartnerID nvarchar(5),@ProducerId int,@PartnerPointID int',@PartnerID=N'92533',@PartnerPointID=48,@ProducerId=102