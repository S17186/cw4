CREATE procedure UpgradeStudentsProcedure 
	@Studies NVARCHAR(100), 
	@Semester INT, 
	@IdEnrollment INT  OUTPUT,
	@StartDate DateTime OUTPUT, 
	@Error INT OUTPUT


AS
BEGIN

	SET XACT_ABORT ON
	BEGIN TRAN

	Declare @ERR NVARCHAR(100);
	SET @Error = 0; 

	--Check if studies exist in DB
	DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies WHERE Name=@Studies) 
	IF @IdStudies IS NULL 
		BEGIN			
			ROLLBACK TRAN
			SET @Error = 1; 
			SET @ERR ='Studies do not exist'; 
			RAISERROR (@ERR,1,1)
			RETURN 
		END

	--Check if enrollment for this studies and semester exist
	DECLARE @Old_IdEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@IdStudies AND Semester=@Semester)
	IF @Old_IdEnrollment IS NULL
		BEGIN
			ROLLBACK TRAN
			SET @Error = 2; 
			SET @ERR = 'No current enrollment for this studies and semester - nothing to upgrade';
			RAISERROR (@ERR,2,2)
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

	COMMIT TRAN

END
