# peopledesk-sass-api

peopledesk-sass-api

### Table Design Guideline:
---
**Data Type**
- BIGINT
- BIT 
- NVARCHAR(50/250/500/3000/MAX)
- NUMERIC(18,6)
- DATETIME
- DATE
- TIME

**Default Field**
- isActive      &emsp;bit       &emsp;not null  &emsp;default(1)
- intCreatedBy	&emsp;bigint	&emsp;not null    
- dteCreatedAt	&emsp;datetime	&emsp;not null  &emsp;default(getdate())
- intUpdatedBy	&emsp;bigint	&emsp;null
- dteUpdatedAt	&emsp;datetime	&emsp;null

### Rules to obey:
---
- developer have to merge with **development** into **dev-branch**
- responsible person will merge with **dev-branch** into **development**
- **scaffold** script will execute only from **development** branch
