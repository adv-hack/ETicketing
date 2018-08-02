/* Seat Colours */

DELETE FROM [dbo].[tbl_stadium_seat_colours]
      WHERE [BUSINESS_UNIT] = 'BOXOFFICE'

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'5'
           ,'A'
           ,'#9ed566'
           ,'#ffffff'
           ,'Available'
           ,'available'
           ,'#9ed566')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'10'
           ,'X'
           ,'#d9534f'
           ,'#ffffff'
           ,'Restricted'
           ,'restricted'
           ,'#d9534f')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'15'
           ,'D'
           ,'#5bc0de'
           ,'#ffffff'
           ,'Disabled Seat'
           ,'disabled-seat'
           ,'#5bc0de')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'20'
           ,'SELECTED'
           ,'#2574ab'
           ,'#ffffff'
           ,'Selected'
           ,'selected'
           ,'#2574ab')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'25'
           ,'B'
           ,'#2574ab'
           ,'#2574ab'
           ,'In Your Basket'
           ,'in-your-basket'
           ,'#2574ab')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'30'
           ,'T'
           ,'#259dab'
           ,'#259dab'
           ,'Transferable Seat'
           ,'transferable-seat'
           ,'#259dab')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'35'
           ,'C'
           ,'#514EA1'
           ,'#514EA1'
           ,'Customer Reserved'
           ,'customer-reserved'
           ,'#514EA1')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'40'
           ,'R'
           ,'#e6ad5c'
           ,'#e6ad5c'
           ,'Reserved'
           ,'reserved'
           ,'#e6ad5c')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'45'
           ,'0.2'
           ,'#909bb1'
           ,'#909bb1'
           ,'Sold'
           ,'sold'
           ,'#909bb1')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'50'
           ,'.'
           ,'#d8dce3'
           ,'#d8dce3'
           ,'Unavailable'
           ,'unavailable '
           ,'#d8dce3')

INSERT INTO [dbo].[tbl_stadium_seat_colours]
           ([BUSINESS_UNIT]
           ,[STADIUM_CODE]
           ,[SEQUENCE]
           ,[SEAT_TYPE]
           ,[OUTLINE_COLOUR]
           ,[FILL_COLOUR]
           ,[TEXT]
           ,[CSS_CLASS]
           ,[DISPLAY_COLOUR])
     VALUES
           ('BOXOFFICE'
           ,'*ALL'
           ,'55'
           ,'HOVER'
           ,'#262b36'
           ,'#ffffff'
           ,'Hover'
           ,'hover '
           ,'#262b36')