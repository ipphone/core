delimiter $$

CREATE TABLE `contacts` (
`id` INT(11)  PRIMARY KEY AUTOINCREMENT NOT NULL,
`first_name` NVARCHAR(255)  NULL,
`last_name` NVARCHAR(255)  NULL,
`middle_name` NVARCHAR(255)  NULL,
`company` NVARCHAR(255)  NULL,
`job_title` NVARCHAR(255)  NULL,
`note` NTEXT  NULL,
`version_tag` INT(11) NOT NULL DEFAULT 1
)
$$

CREATE TABLE `contact_phones` (
`id` INT(11)  PRIMARY KEY AUTOINCREMENT NOT NULL,
`contact_id` INT(11) NOT NULL,
`number` NVARCHAR(50) NOT NULL,
`comment` NVARCHAR(255) NULL,
`version_tag` INT(11) NOT NULL DEFAULT 1
)
$$

CREATE TABLE `contact_emails` (
`id` INT(11)  PRIMARY KEY AUTOINCREMENT NOT NULL,
`contact_id` INT(11) NOT NULL,
`key` NVARCHAR(50)  NULL,
`email` NVARCHAR(50) NOT NULL,
`comment` NVARCHAR(255) NULL,
`version_tag` INT(11) NOT NULL DEFAULT 1
)
$$

CREATE TABLE `tags` (
`id` INT(11) PRIMARY KEY AUTOINCREMENT NOT NULL,
`version_tag` INT(11) NOT NULL DEFAULT 1,
`name` NVARCHAR(50) NOT NULL,
`color` NVARCHAR(16) DEFAULT '' NOT NULL
)
$$

CREATE TABLE `tags_links` (
`tag_id` INT(11) NOT NULL,
`contact_id` INT(11) NOT NULL
)
$$
