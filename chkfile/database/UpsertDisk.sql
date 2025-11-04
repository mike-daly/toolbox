
CREATE PROCEDURE UpsertMyTable (
    IN p_DeviceID        VARCHAR(255),
    IN p_PNPDeviceID     VARCHAR(255),
    IN p_Model           VARCHAR(64),
    IN p_InterfaceType   VARCHAR(32),
    IN p_MediaType       VARCHAR(64),
    IN p_Size            BIGINT,
    IN p_Partitions      INT,
    IN p_SerialNumber    VARCHAR(255),
    IN p_Status          VARCHAR(64)

)
BEGIN
    INSERT INTO disk (

        DeviceID,
        PNPDeviceID,
        Model,
        InterfaceType,
        MediaType,
        Size,
        Partitions,
        SerialNumber,
        Status

    ) VALUES (
        p_DeviceID,
        p_PNPDeviceID,
        p_Model,
        p_InterfaceType,
        p_MediaType,
        p_Size,
        p_Partitions,
        p_SerialNumber,
        p_Status
    )
    ON DUPLICATE KEY UPDATE

        DeviceID = p_DeviceID,
        PNPDeviceID = p_PNPDeviceID,
        Model = p_Model,
        InterfaceType = p_InterfaceType,
        MediaType = p_MediaType,
        Size = p_Size,
        Partitions = p_Partitions,
        SerialNumber = p_SerialNumber,
        Status = p_Status

END
