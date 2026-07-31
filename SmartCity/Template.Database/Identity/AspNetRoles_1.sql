/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

declare @adminRoleId nvarchar(450) = (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [Name] = 'Admin')
if(@adminRoleId is null)
begin
	INSERT into [dbo].[AspNetRoles] ([Id], [Name], [ConcurrencyStamp], [NormalizedName]) VALUES (N'b246e01a-c313-4586-8fa9-349febfff720', N'Admin', N'7e32a625-f24c-43bb-8210-8ee2c85ffb7c', N'ADMIN')
end
