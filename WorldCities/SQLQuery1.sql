delete from Countries;
delete from Cities;
select * from Countries where Name='South Georgia and South Sandwich Islands'
select count(*) from Cities
select TOP(100) c.*, ct.Name from Cities c, Countries ct Where ct.Id=c.CountryId