alter procedure UpgradeStudentsProcedure 
	@Studies NVARCHAR(100), 
	@Semester INT, 
	@IdEnrollment INT = -2,
	@StartDate DateTime = null OUTPUT 

AS
BEGIN

	SET XACT_ABORT ON
	BEGIN TRAN

	--Check if studies exist in DB
	DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies WHERE Name=@Studies) 
	IF @IdStudies IS NULL 
	BEGIN
		RAISERROR ('Studies do not exist',1,1)
		ROLLBACK
		RETURN
	END

	--Check if enrollment for this studies and semester exist
	DECLARE @Old_IdEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@IdStudies AND Semester=@Semester)
	IF @Old_IdEnrollment IS NULL
		BEGIN
			RAISERROR ('No current enrollment for this studies and semester - nothing to upgrade',1,1)
			ROLLBACK
			RETURN
		END
	
	--Check if enrollment for the next semester exists for the given studies
	--	if not, create new with today's date 
	SET @IdEnrollment = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@IdStudies AND Semester=@Semester+1)
	IF @IdEnrollment IS NULL
		BEGIN
			--CREATE NEW
			SET @IdEnrollment = (select max(idEnrollment) from Enrollment)+1
			INSERT INTO Enrollment VALUES ( @IdEnrollment, @Semester+1, @IdStudies, (SELECT GETDATE()))
		END

	-- UPGRADE students
	UPDATE Student 
	SET IdEnrollment=@IdEnrollment
	WHERE IdEnrollment=@Old_IdEnrollment 

	-- Return PARAMS
	SET @StartDate = (Select StartDate from Enrollment where IdEnrollment=@IdEnrollment)

	COMMIT

	return @IdEnrollment
END
	