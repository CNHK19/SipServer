/*****************************************************************************
*
* Copyright (c) Microsoft Corporation.  All rights reserved.
*
* Module Name:
*
*    UccApiErr.mc
*
* Abstract:
*
*    Error Messages for UCC API
*
*****************************************************************************/
// NOTE: We should use SEVERITY_SUCCESS and SEVERITY_ERRROR, but
//       "mc.exe -o" doesn't shift the severity 31 bits.  It only
//       shifts it 30 bits.
/////////////////////////////////////////////////////////////////////////
// ERROR CODE DEFINITIONS
//
// don't insert event tracing strings here; use UccpEvtLog.mc
/////////////////////////////////////////////////////////////////////////
// Possible error codes from SIP interfaces
//
//  Values are 32 bit values laid out as follows:
//
//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
//  +-+-+-+-+-+---------------------+-------------------------------+
//  |S|R|C|N|r|    Facility         |               Code            |
//  +-+-+-+-+-+---------------------+-------------------------------+
//
//  where
//
//      S - Severity - indicates success/fail
//
//          0 - Success
//          1 - Fail (COERROR)
//
//      R - reserved portion of the facility code, corresponds to NT's
//              second severity bit.
//
//      C - reserved portion of the facility code, corresponds to NT's
//              C field.
//
//      N - reserved portion of the facility code. Used to indicate a
//              mapped NT status value.
//
//      r - reserved portion of the facility code. Reserved for internal
//              use. Used to indicate HRESULT values that are not status
//              values, but are instead message ids for display strings.
//
//      Facility - is the facility code
//
//      Code - is the facility's status code
//
//
// Define the facility codes
//
#define FACILITY_UCC_INTERFACE           0xEE
#define FACILITY_SIP_INTERFACE           0xEF


//
// Define the severity codes
//
#define STATUS_SEVERITY_COERROR          0x2
#define STATUS_SEVERITY_SUCCESS          0x0


//
// MessageId: UCC_E_CODECS_MISMATCH
//
// MessageText:
//
// No matching codecs with peer.
//
#define UCC_E_CODECS_MISMATCH            ((HRESULT)0x80EE0000L)

//
// MessageId: UCC_E_STREAM_PRESENT
//
// MessageText:
//
// The stream to be started is already present.
//
#define UCC_E_STREAM_PRESENT             ((HRESULT)0x80EE0001L)

//
// MessageId: UCC_E_STREAM_NOT_PRESENT
//
// MessageText:
//
// The stream to be stopped is not present.
//
#define UCC_E_STREAM_NOT_PRESENT         ((HRESULT)0x80EE0002L)

//
// MessageId: UCC_E_NO_STREAM
//
// MessageText:
//
// No stream is active.
//
#define UCC_E_NO_STREAM                  ((HRESULT)0x80EE0003L)

//
// MessageId: UCC_E_SIP_PARSE_FAILED
//
// MessageText:
//
// Parsing SIP failed.
//
#define UCC_E_SIP_PARSE_FAILED           ((HRESULT)0x80EE0004L)

//
// MessageId: UCC_E_SIP_HEADER_NOT_PRESENT
//
// MessageText:
//
// The SIP header is not present in the message.
//
#define UCC_E_SIP_HEADER_NOT_PRESENT     ((HRESULT)0x80EE0005L)

//
// MessageId: UCC_E_SDP_NOT_PRESENT
//
// MessageText:
//
// SDP is not present in the SIP message.
//
#define UCC_E_SDP_NOT_PRESENT            ((HRESULT)0x80EE0006L)

//
// MessageId: UCC_E_SDP_PARSE_FAILED
//
// MessageText:
//
// Parsing SDP failed.
//
#define UCC_E_SDP_PARSE_FAILED           ((HRESULT)0x80EE0007L)

//
// MessageId: UCC_E_SDP_UPDATE_FAILED
//
// MessageText:
//
// SDP does not match the previous one.
//
#define UCC_E_SDP_UPDATE_FAILED          ((HRESULT)0x80EE0008L)

//
// MessageId: UCC_E_SDP_MULTICAST
//
// MessageText:
//
// Multicast is not supported.
//
#define UCC_E_SDP_MULTICAST              ((HRESULT)0x80EE0009L)

//
// MessageId: UCC_E_SDP_CONNECTION_ADDR
//
// MessageText:
//
// Media does not contain connection address.
//
#define UCC_E_SDP_CONNECTION_ADDR        ((HRESULT)0x80EE000AL)

//
// MessageId: UCC_E_SDP_NO_MEDIA
//
// MessageText:
//
// No media is available for the session.
//
#define UCC_E_SDP_NO_MEDIA               ((HRESULT)0x80EE000BL)

//
// MessageId: UCC_E_SIP_TIMEOUT
//
// MessageText:
//
// SIP transaction timed out.
//
#define UCC_E_SIP_TIMEOUT                ((HRESULT)0x80EE000CL)

//
// MessageId: UCC_E_SDP_FAILED_TO_BUILD
//
// MessageText:
//
// Failed to build SDP blob.
//
#define UCC_E_SDP_FAILED_TO_BUILD        ((HRESULT)0x80EE000DL)

//
// MessageId: UCC_E_SIP_INVITE_TRANSACTION_PENDING
//
// MessageText:
//
// Currently processing another INVITE transaction.
//
#define UCC_E_SIP_INVITE_TRANSACTION_PENDING ((HRESULT)0x80EE000EL)

//
// MessageId: UCC_E_SIP_AUTHENTICATION_HEADER_SENT
//
// MessageText:
//
// Authorization header was sent in a previous request.
//
#define UCC_E_SIP_AUTHENTICATION_HEADER_SENT ((HRESULT)0x80EE000FL)

//
// MessageId: UCC_E_SIP_AUTHENTICATION_TYPE_NOT_SUPPORTED
//
// MessageText:
//
// The authentication type requested is not supported.
//
#define UCC_E_SIP_AUTHENTICATION_TYPE_NOT_SUPPORTED ((HRESULT)0x80EE0010L)

//
// MessageId: UCC_E_SIP_AUTHENTICATION_FAILED
//
// MessageText:
//
// Authentication failed.
//
#define UCC_E_SIP_AUTHENTICATION_FAILED  ((HRESULT)0x80EE0011L)

//
// MessageId: UCC_E_INVALID_SIP_URL
//
// MessageText:
//
// The SIP URL is not valid.
//
#define UCC_E_INVALID_SIP_URL            ((HRESULT)0x80EE0012L)

//
// MessageId: UCC_E_DESTINATION_ADDRESS_LOCAL
//
// MessageText:
//
// The destination address belongs to the local machine.
//
#define UCC_E_DESTINATION_ADDRESS_LOCAL  ((HRESULT)0x80EE0013L)

//
// MessageId: UCC_E_INVALID_ADDRESS_LOCAL
//
// MessageText:
//
// The local address is invalid. Check the endpoint.
//
#define UCC_E_INVALID_ADDRESS_LOCAL      ((HRESULT)0x80EE0014L)

//
// MessageId: UCC_E_DESTINATION_ADDRESS_MULTICAST
//
// MessageText:
//
// The destination address is a multicast address.
//
#define UCC_E_DESTINATION_ADDRESS_MULTICAST ((HRESULT)0x80EE0015L)

//
// MessageId: UCC_E_INVALID_PROXY_ADDRESS
//
// MessageText:
//
// The proxy address is not valid.
//
#define UCC_E_INVALID_PROXY_ADDRESS      ((HRESULT)0x80EE0016L)

//
// MessageId: UCC_E_SIP_TRANSPORT_NOT_SUPPORTED
//
// MessageText:
//
// The specified transport is not supported.
//
#define UCC_E_SIP_TRANSPORT_NOT_SUPPORTED ((HRESULT)0x80EE0017L)

//
// MessageId: UCC_E_SIP_NEED_MORE_DATA
//
// MessageText:
//
// Need more data for parsing a whole SIP message.
//
#define UCC_E_SIP_NEED_MORE_DATA         ((HRESULT)0x80EE0018L)

//
// MessageId: UCC_E_SIP_CALL_DISCONNECTED
//
// MessageText:
//
// The call has been disconnected.
//
#define UCC_E_SIP_CALL_DISCONNECTED      ((HRESULT)0x80EE0019L)

//
// MessageId: UCC_E_SIP_REQUEST_DESTINATION_ADDR_NOT_PRESENT
//
// MessageText:
//
// The request destination address is not known.
//
#define UCC_E_SIP_REQUEST_DESTINATION_ADDR_NOT_PRESENT ((HRESULT)0x80EE001AL)

//
// MessageId: UCC_E_SIP_UDP_SIZE_EXCEEDED
//
// MessageText:
//
// The sip message size is greater than the UDP message size allowed.
//
#define UCC_E_SIP_UDP_SIZE_EXCEEDED      ((HRESULT)0x80EE001BL)

//
// MessageId: UCC_E_SIP_SSL_TUNNEL_FAILED
//
// MessageText:
//
// Cannot establish SSL tunnel to HTTP proxy.
//
#define UCC_E_SIP_SSL_TUNNEL_FAILED      ((HRESULT)0x80EE001CL)

//
// MessageId: UCC_E_SIP_SSL_NEGOTIATION_TIMEOUT
//
// MessageText:
//
// Timeout during SSL negotiation.
//
#define UCC_E_SIP_SSL_NEGOTIATION_TIMEOUT ((HRESULT)0x80EE001DL)

//
// MessageId: UCC_E_SIP_STACK_SHUTDOWN
//
// MessageText:
//
// Sip stack is already shutdown.
//
#define UCC_E_SIP_STACK_SHUTDOWN         ((HRESULT)0x80EE001EL)

// media error codes
//
// MessageId: UCC_E_MEDIA_CONTROLLER_STATE
//
// MessageText:
//
// Operation not allowed in current media controller state.
//
#define UCC_E_MEDIA_CONTROLLER_STATE     ((HRESULT)0x80EE001FL)

//
// MessageId: UCC_E_MEDIA_NEED_TERMINAL
//
// MessageText:
//
// Can not find device.
//
#define UCC_E_MEDIA_NEED_TERMINAL        ((HRESULT)0x80EE0020L)

//
// MessageId: UCC_E_MEDIA_AUDIO_DEVICE_NOT_AVAILABLE
//
// MessageText:
//
// Audio device is not available.
//
#define UCC_E_MEDIA_AUDIO_DEVICE_NOT_AVAILABLE ((HRESULT)0x80EE0021L)

//
// MessageId: UCC_E_MEDIA_VIDEO_DEVICE_NOT_AVAILABLE
//
// MessageText:
//
// Video device is not available.
//
#define UCC_E_MEDIA_VIDEO_DEVICE_NOT_AVAILABLE ((HRESULT)0x80EE0022L)

//
// MessageId: UCC_E_START_STREAM
//
// MessageText:
//
// Can not start stream.
//
#define UCC_E_START_STREAM               ((HRESULT)0x80EE0023L)

//
// MessageId: UCC_E_MEDIA_AEC
//
// MessageText:
//
// Failed to enable acoustic echo cancellation.
//
#define UCC_E_MEDIA_AEC                  ((HRESULT)0x80EE0024L)

// Core error codes
//
// MessageId: UCC_E_PLATFORM_NOT_INITIALIZED
//
// MessageText:
//
// Client not initialized.
//
#define UCC_E_PLATFORM_NOT_INITIALIZED   ((HRESULT)0x80EE0025L)

//
// MessageId: UCC_E_PLATFORM_ALREADY_INITIALIZED
//
// MessageText:
//
// Client already initialized.
//
#define UCC_E_PLATFORM_ALREADY_INITIALIZED ((HRESULT)0x80EE0026L)

//
// MessageId: UCC_E_PLATFORM_ALREADY_SHUT_DOWN
//
// MessageText:
//
// Client already shut down.
//
#define UCC_E_PLATFORM_ALREADY_SHUT_DOWN ((HRESULT)0x80EE0027L)

//
// MessageId: UCC_E_PRESENCE_NOT_ENABLED
//
// MessageText:
//
// Presence not enabled.
//
#define UCC_E_PRESENCE_NOT_ENABLED       ((HRESULT)0x80EE0028L)

//
// MessageId: UCC_E_INVALID_SESSION_TYPE
//
// MessageText:
//
// Invalid session type.
//
#define UCC_E_INVALID_SESSION_TYPE       ((HRESULT)0x80EE0029L)

//
// MessageId: UCC_E_INVALID_SESSION_STATE
//
// MessageText:
//
// Invalid session state.
//
#define UCC_E_INVALID_SESSION_STATE      ((HRESULT)0x80EE002AL)

//
// MessageId: UCC_E_NO_ENDPOINT
//
// MessageText:
//
// No valid endpoint for this operation.
//
#define UCC_E_NO_ENDPOINT                ((HRESULT)0x80EE002BL)

//
// MessageId: UCC_E_LOCAL_PHONE_NEEDED
//
// MessageText:
//
// A local phone number is needed.
//
#define UCC_E_LOCAL_PHONE_NEEDED         ((HRESULT)0x80EE002CL)

//
// MessageId: UCC_E_NO_DEVICE
//
// MessageText:
//
// No preferred device.
//
#define UCC_E_NO_DEVICE                  ((HRESULT)0x80EE002DL)

//
// MessageId: UCC_E_INVALID_ENDPOINT
//
// MessageText:
//
// Invalid endpoint.
//
#define UCC_E_INVALID_ENDPOINT           ((HRESULT)0x80EE002EL)

//
// MessageId: UCC_E_ENDPOINT_NO_PROVISION
//
// MessageText:
//
// No provision tag in endpoint.
//
#define UCC_E_ENDPOINT_NO_PROVISION      ((HRESULT)0x80EE002FL)

//
// MessageId: UCC_E_ENDPOINT_NO_KEY
//
// MessageText:
//
// No endpoint key.
//
#define UCC_E_ENDPOINT_NO_KEY            ((HRESULT)0x80EE0030L)

//
// MessageId: UCC_E_ENDPOINT_NO_NAME
//
// MessageText:
//
// No endpoint name.
//
#define UCC_E_ENDPOINT_NO_NAME           ((HRESULT)0x80EE0031L)

//
// MessageId: UCC_E_ENDPOINT_NO_USER
//
// MessageText:
//
// No user tag in endpoint.
//
#define UCC_E_ENDPOINT_NO_USER           ((HRESULT)0x80EE0032L)

//
// MessageId: UCC_E_ENDPOINT_NO_USER_URI
//
// MessageText:
//
// No user URI in endpoint.
//
#define UCC_E_ENDPOINT_NO_USER_URI       ((HRESULT)0x80EE0033L)

//
// MessageId: UCC_E_ENDPOINT_NO_SERVER
//
// MessageText:
//
// No server tag in endpoint.
//
#define UCC_E_ENDPOINT_NO_SERVER         ((HRESULT)0x80EE0034L)

//
// MessageId: UCC_E_ENDPOINT_NO_SERVER_ADDRESS
//
// MessageText:
//
// Server tag missing address in endpoint.
//
#define UCC_E_ENDPOINT_NO_SERVER_ADDRESS ((HRESULT)0x80EE0035L)

//
// MessageId: UCC_E_ENDPOINT_NO_SERVER_PROTOCOL
//
// MessageText:
//
// Server tag missing protocol in endpoint.
//
#define UCC_E_ENDPOINT_NO_SERVER_PROTOCOL ((HRESULT)0x80EE0036L)

//
// MessageId: UCC_E_ENDPOINT_INVALID_SERVER_PROTOCOL
//
// MessageText:
//
// Invalid server protocol in endpoint.
//
#define UCC_E_ENDPOINT_INVALID_SERVER_PROTOCOL ((HRESULT)0x80EE0037L)

//
// MessageId: UCC_E_ENDPOINT_INVALID_SERVER_AUTHENTICATION_MODE
//
// MessageText:
//
// Invalid server authentication method in endpoint.
//
#define UCC_E_ENDPOINT_INVALID_SERVER_AUTHENTICATION_MODE ((HRESULT)0x80EE0038L)

//
// MessageId: UCC_E_ENDPOINT_INVALID_SERVER_ROLE
//
// MessageText:
//
// Invalid server role in endpoint.
//
#define UCC_E_ENDPOINT_INVALID_SERVER_ROLE ((HRESULT)0x80EE0039L)

//
// MessageId: UCC_E_ENDPOINT_MULTIPLE_REGISTRARS
//
// MessageText:
//
// Multiple registrar servers in endpoint.
//
#define UCC_E_ENDPOINT_MULTIPLE_REGISTRARS ((HRESULT)0x80EE003AL)

//
// MessageId: UCC_E_ENDPOINT_INVALID_SESSION
//
// MessageText:
//
// Invalid session tag in endpoint.
//
#define UCC_E_ENDPOINT_INVALID_SESSION   ((HRESULT)0x80EE003BL)

//
// MessageId: UCC_E_ENDPOINT_INVALID_SESSION_PARTY
//
// MessageText:
//
// Invalid session party in endpoint.
//
#define UCC_E_ENDPOINT_INVALID_SESSION_PARTY ((HRESULT)0x80EE003CL)

//
// MessageId: UCC_E_ENDPOINT_INVALID_SESSION_TYPE
//
// MessageText:
//
// Invalid session type in endpoint.
//
#define UCC_E_ENDPOINT_INVALID_SESSION_TYPE ((HRESULT)0x80EE003DL)

//
// MessageId: UCC_E_OPERATION_WITH_TOO_MANY_PARTICIPANTS
//
// MessageText:
//
// The operation failed because of too many participants in the session.
//
#define UCC_E_OPERATION_WITH_TOO_MANY_PARTICIPANTS ((HRESULT)0x80EE003EL)

//
// MessageId: UCC_E_BASIC_AUTHENTICATION_SET_TLS
//
// MessageText:
//
// Must set transport to TLS if Basic Auth is allowed.
//
#define UCC_E_BASIC_AUTHENTICATION_SET_TLS ((HRESULT)0x80EE003FL)

//
// MessageId: UCC_E_SIP_HIGH_SECURITY_SET_TLS
//
// MessageText:
//
// Must set transport to TLS if high security mode is needed.
//
#define UCC_E_SIP_HIGH_SECURITY_SET_TLS  ((HRESULT)0x80EE0040L)

//
// MessageId: UCC_S_ROAMING_NOT_SUPPORTED
//
// MessageText:
//
// Server does not support this type of roaming.
//
#define UCC_S_ROAMING_NOT_SUPPORTED      ((HRESULT)0x00EE0041L)

//
// MessageId: UCC_E_ENDPOINT_SERVER_UNAUTHORIZED
//
// MessageText:
//
// Server address does not match an authorized domain in endpoint.
//
#define UCC_E_ENDPOINT_SERVER_UNAUTHORIZED ((HRESULT)0x80EE0042L)

//
// MessageId: UCC_E_DUPLICATE_REALM
//
// MessageText:
//
// Duplicate realm exists in an enabled endpoint.
//
#define UCC_E_DUPLICATE_REALM            ((HRESULT)0x80EE0043L)

//
// MessageId: UCC_E_POLICY_NOT_ALLOW
//
// MessageText:
//
// Current policy settings do not allow this action.
//
#define UCC_E_POLICY_NOT_ALLOW           ((HRESULT)0x80EE0044L)

//
// MessageId: UCC_E_PORT_MAPPING_UNAVAILABLE
//
// MessageText:
//
// Port mapping can not be obtained from the port manager.
//
#define UCC_E_PORT_MAPPING_UNAVAILABLE   ((HRESULT)0x80EE0045L)

//
// MessageId: UCC_E_PORT_MAPPING_FAILED
//
// MessageText:
//
// Port mapping failure returned from the port manager.
//
#define UCC_E_PORT_MAPPING_FAILED        ((HRESULT)0x80EE0046L)

//
// MessageId: UCC_E_SECURITY_LEVEL_NOT_COMPATIBLE
//
// MessageText:
//
// The local and remote security levels are not compatible.
//
#define UCC_E_SECURITY_LEVEL_NOT_COMPATIBLE ((HRESULT)0x80EE0047L)

//
// MessageId: UCC_E_SECURITY_LEVEL_NOT_DEFINED
//
// MessageText:
//
// The security level is not defined.
//
#define UCC_E_SECURITY_LEVEL_NOT_DEFINED ((HRESULT)0x80EE0048L)

//
// MessageId: UCC_E_SECURITY_LEVEL_NOT_SUPPORTED_BY_PARTICIPANT
//
// MessageText:
//
// Participant could not support the requested security level.
//
#define UCC_E_SECURITY_LEVEL_NOT_SUPPORTED_BY_PARTICIPANT ((HRESULT)0x80EE0049L)

//
// MessageId: UCC_E_DUPLICATE_BUDDY
//
// MessageText:
//
// Contact already exists.
//
#define UCC_E_DUPLICATE_BUDDY            ((HRESULT)0x80EE004AL)

//
// MessageId: UCC_E_DUPLICATE_WATCHER
//
// MessageText:
//
// Watcher already exists.
//
#define UCC_E_DUPLICATE_WATCHER          ((HRESULT)0x80EE004BL)

//
// MessageId: UCC_E_MALFORMED_XML
//
// MessageText:
//
// Malformed XML.
//
#define UCC_E_MALFORMED_XML              ((HRESULT)0x80EE004CL)

//
// MessageId: UCC_E_ROAMING_OPERATION_INTERRUPTED
//
// MessageText:
//
// Roaming operation interrupted. It may still succeed or fail.
//
#define UCC_E_ROAMING_OPERATION_INTERRUPTED ((HRESULT)0x80EE004DL)

//
// MessageId: UCC_E_ROAMING_FAILED
//
// MessageText:
//
// Roaming session failed.
//
#define UCC_E_ROAMING_FAILED             ((HRESULT)0x80EE004EL)

//
// MessageId: UCC_E_INVALID_BUDDY_LIST
//
// MessageText:
//
// Contact list is invalid.
//
#define UCC_E_INVALID_BUDDY_LIST         ((HRESULT)0x80EE004FL)

//
// MessageId: UCC_E_INVALID_ACL_LIST
//
// MessageText:
//
// ACL list is invalid.
//
#define UCC_E_INVALID_ACL_LIST           ((HRESULT)0x80EE0050L)

//
// MessageId: UCC_E_NO_GROUP
//
// MessageText:
//
// Group does not exist.
//
#define UCC_E_NO_GROUP                   ((HRESULT)0x80EE0051L)

//
// MessageId: UCC_E_DUPLICATE_GROUP
//
// MessageText:
//
// Group already exists.
//
#define UCC_E_DUPLICATE_GROUP            ((HRESULT)0x80EE0052L)

//
// MessageId: UCC_E_TOO_MANY_GROUPS
//
// MessageText:
//
// Max number of groups has been reached.
//
#define UCC_E_TOO_MANY_GROUPS            ((HRESULT)0x80EE0053L)

//
// MessageId: UCC_E_NO_BUDDY
//
// MessageText:
//
// Contact does not exist.
//
#define UCC_E_NO_BUDDY                   ((HRESULT)0x80EE0054L)

//
// MessageId: UCC_E_NO_WATCHER
//
// MessageText:
//
// Watcher does not exist.
//
#define UCC_E_NO_WATCHER                 ((HRESULT)0x80EE0055L)

//
// MessageId: UCC_E_NO_REALM
//
// MessageText:
//
// No realm is set.
//
#define UCC_E_NO_REALM                   ((HRESULT)0x80EE0056L)

//
// MessageId: UCC_E_NO_TRANSPORT
//
// MessageText:
//
// Server can not be specified without a transport protocol.
//
#define UCC_E_NO_TRANSPORT               ((HRESULT)0x80EE0057L)

//
// MessageId: UCC_E_NOT_EXIST
//
// MessageText:
//
// The required item does not exist.
//
#define UCC_E_NOT_EXIST                  ((HRESULT)0x80EE0058L)

//
// MessageId: UCC_E_INVALID_PREFERENCE_LIST
//
// MessageText:
//
// Preference list is invalid.
//
#define UCC_E_INVALID_PREFERENCE_LIST    ((HRESULT)0x80EE0059L)

//
// MessageId: UCC_E_MAX_PENDING_OPERATIONS
//
// MessageText:
//
// Maximum number of pending operations reached.
//
#define UCC_E_MAX_PENDING_OPERATIONS     ((HRESULT)0x80EE005AL)

//
// MessageId: UCC_E_TOO_MANY_RETRIES
//
// MessageText:
//
// Too many attempts to resend a request.
//
#define UCC_E_TOO_MANY_RETRIES           ((HRESULT)0x80EE005BL)

//
// MessageId: UCC_E_INVALID_PORTRANGE
//
// MessageText:
//
// Invalid port range.
//
#define UCC_E_INVALID_PORTRANGE          ((HRESULT)0x80EE005CL)

//
// MessageId: UCC_E_SIP_CALL_CONNECTION_NOT_ESTABLISHED
//
// MessageText:
//
// Call connection has not been established.
//
#define UCC_E_SIP_CALL_CONNECTION_NOT_ESTABLISHED ((HRESULT)0x80EE005DL)

//
// MessageId: UCC_E_SIP_ADDITIONAL_PARTY_IN_TWO_PARTY_SESSION
//
// MessageText:
//
// Adding additional parties to two party session failed.
//
#define UCC_E_SIP_ADDITIONAL_PARTY_IN_TWO_PARTY_SESSION ((HRESULT)0x80EE005EL)

//
// MessageId: UCC_E_SIP_PARTY_ALREADY_IN_SESSION
//
// MessageText:
//
// Party already exists in session.
//
#define UCC_E_SIP_PARTY_ALREADY_IN_SESSION ((HRESULT)0x80EE005FL)

//
// MessageId: UCC_E_SIP_OTHER_PARTY_JOIN_IN_PROGRESS
//
// MessageText:
//
// Join operation is in progress for another party.
//
#define UCC_E_SIP_OTHER_PARTY_JOIN_IN_PROGRESS ((HRESULT)0x80EE0060L)

//
// MessageId: UCC_E_INVALID_OBJECT_STATE
//
// MessageText:
//
// Object state does not allow to perform this operation.
//
#define UCC_E_INVALID_OBJECT_STATE       ((HRESULT)0x80EE0061L)

//
// MessageId: UCC_E_PRESENCE_ENABLED
//
// MessageText:
//
// Presence is enabled.
//
#define UCC_E_PRESENCE_ENABLED           ((HRESULT)0x80EE0062L)

//
// MessageId: UCC_E_ROAMING_ENABLED
//
// MessageText:
//
// Roaming is enabled.
//
#define UCC_E_ROAMING_ENABLED            ((HRESULT)0x80EE0063L)

//
// MessageId: UCC_E_SIP_TLS_INCOMPATIBLE_ENCRYPTION
//
// MessageText:
//
// Incompatible TLS encryption.
//
#define UCC_E_SIP_TLS_INCOMPATIBLE_ENCRYPTION ((HRESULT)0x80EE0064L)

//
// MessageId: UCC_E_SIP_INVALID_CERTIFICATE
//
// MessageText:
//
// Invalid certificate.
//
#define UCC_E_SIP_INVALID_CERTIFICATE    ((HRESULT)0x80EE0065L)

//
// MessageId: UCC_E_SIP_DNS_FAIL
//
// MessageText:
//
// DNS lookup fails.
//
#define UCC_E_SIP_DNS_FAIL               ((HRESULT)0x80EE0066L)

//
// MessageId: UCC_E_SIP_TCP_FAIL
//
// MessageText:
//
// Fails to make a TCP connection.
//
#define UCC_E_SIP_TCP_FAIL               ((HRESULT)0x80EE0067L)

//
// MessageId: UCC_E_TOO_SMALL_EXPIRES_VALUE
//
// MessageText:
//
// Expiration value received from the server is too small.
//
#define UCC_E_TOO_SMALL_EXPIRES_VALUE    ((HRESULT)0x80EE0068L)

//
// MessageId: UCC_E_SIP_TLS_FAIL
//
// MessageText:
//
// Fails to make a TLS connection.
//
#define UCC_E_SIP_TLS_FAIL               ((HRESULT)0x80EE0069L)

//
// MessageId: UCC_E_NOT_PRESENCE_ENDPOINT
//
// MessageText:
//
// A presence endpoint must be used.
//
#define UCC_E_NOT_PRESENCE_ENDPOINT      ((HRESULT)0x80EE006AL)

//
// MessageId: UCC_E_SIP_INVITEE_PARTY_TIMEOUT
//
// MessageText:
//
// Invitee connection fails.
//
#define UCC_E_SIP_INVITEE_PARTY_TIMEOUT  ((HRESULT)0x80EE006BL)

//
// MessageId: UCC_E_SIP_AUTHENTICATION_TIME_SKEW
//
// MessageText:
//
// Authentication failure because of time skew between client and server.
//
#define UCC_E_SIP_AUTHENTICATION_TIME_SKEW ((HRESULT)0x80EE006CL)

//
// MessageId: UCC_E_INVALID_REGISTRATION_STATE
//
// MessageText:
//
// Invalid registration state.
//
#define UCC_E_INVALID_REGISTRATION_STATE ((HRESULT)0x80EE006DL)

//
// MessageId: UCC_E_MEDIA_DISABLED
//
// MessageText:
//
// Media is disabled.
//
#define UCC_E_MEDIA_DISABLED             ((HRESULT)0x80EE006EL)

//
// MessageId: UCC_E_MEDIA_ENABLED
//
// MessageText:
//
// Media is enabled.
//
#define UCC_E_MEDIA_ENABLED              ((HRESULT)0x80EE006FL)

//
// MessageId: UCC_E_REFER_NOT_ACCEPTED
//
// MessageText:
//
// Refer has not been accepted.
//
#define UCC_E_REFER_NOT_ACCEPTED         ((HRESULT)0x80EE0070L)

//
// MessageId: UCC_E_REFER_NOT_ALLOWED
//
// MessageText:
//
// Refer operation is not allowed in this session.
//
#define UCC_E_REFER_NOT_ALLOWED          ((HRESULT)0x80EE0071L)

//
// MessageId: UCC_E_REFER_NOT_EXIST
//
// MessageText:
//
// Refer session does not exist or has finished.
//
#define UCC_E_REFER_NOT_EXIST            ((HRESULT)0x80EE0072L)

//
// MessageId: UCC_E_SIP_HOLD_OPERATION_PENDING
//
// MessageText:
//
// Currently a hold operation is pending.
//
#define UCC_E_SIP_HOLD_OPERATION_PENDING ((HRESULT)0x80EE0073L)

//
// MessageId: UCC_E_SIP_UNHOLD_OPERATION_PENDING
//
// MessageText:
//
// Currently an unhold operation is pending.
//
#define UCC_E_SIP_UNHOLD_OPERATION_PENDING ((HRESULT)0x80EE0074L)

//
// MessageId: UCC_E_MEDIA_SESSION_NOT_EXIST
//
// MessageText:
//
// Media session does not exist.
//
#define UCC_E_MEDIA_SESSION_NOT_EXIST    ((HRESULT)0x80EE0075L)

//
// MessageId: UCC_E_MEDIA_SESSION_IN_HOLD
//
// MessageText:
//
// Media session is in hold.
//
#define UCC_E_MEDIA_SESSION_IN_HOLD      ((HRESULT)0x80EE0076L)

//
// MessageId: UCC_E_ANOTHER_MEDIA_SESSION_ACTIVE
//
// MessageText:
//
// Another media session is active.
//
#define UCC_E_ANOTHER_MEDIA_SESSION_ACTIVE ((HRESULT)0x80EE0077L)

//
// MessageId: UCC_E_MAX_REDIRECTS
//
// MessageText:
//
// Too many redirects.
//
#define UCC_E_MAX_REDIRECTS              ((HRESULT)0x80EE0078L)

//
// MessageId: UCC_E_REDIRECT_PROCESSING_FAILED
//
// MessageText:
//
// Processing redirect failed.
//
#define UCC_E_REDIRECT_PROCESSING_FAILED ((HRESULT)0x80EE0079L)

//
// MessageId: UCC_E_LISTENING_SOCKET_NOT_EXIST
//
// MessageText:
//
// Listening socket does not exist.
//
#define UCC_E_LISTENING_SOCKET_NOT_EXIST ((HRESULT)0x80EE007AL)

//
// MessageId: UCC_E_INVALID_LISTEN_SOCKET
//
// MessageText:
//
// Specified address and port is invalid.
//
#define UCC_E_INVALID_LISTEN_SOCKET      ((HRESULT)0x80EE007BL)

//
// MessageId: UCC_E_PORT_MANAGER_ALREADY_SET
//
// MessageText:
//
// Port manager already set.
//
#define UCC_E_PORT_MANAGER_ALREADY_SET   ((HRESULT)0x80EE007CL)

//
// MessageId: UCC_E_SECURITY_LEVEL_ALREADY_SET
//
// MessageText:
//
// The security level has already been set for this Media type can and can not be changed.
//
#define UCC_E_SECURITY_LEVEL_ALREADY_SET ((HRESULT)0x80EE007DL)

//
// MessageId: UCC_E_UDP_NOT_SUPPORTED
//
// MessageText:
//
// This feature is not supported when one of the server in endpoint has UDP transport.
//
#define UCC_E_UDP_NOT_SUPPORTED          ((HRESULT)0x80EE007EL)

//
// MessageId: UCC_E_SIP_REFER_OPERATION_PENDING
//
// MessageText:
//
// Currently a refer operation is pending.
//
#define UCC_E_SIP_REFER_OPERATION_PENDING ((HRESULT)0x80EE007FL)

//
// MessageId: UCC_E_PLATFORM_NOT_SUPPORTED
//
// MessageText:
//
// This operation is not supported on this Windows platform.
//
#define UCC_E_PLATFORM_NOT_SUPPORTED     ((HRESULT)0x80EE0080L)

//
// MessageId: UCC_E_SIP_PEER_PARTICIPANT_IN_MULTIPARTY_SESSION
//
// MessageText:
//
// A peer participant cannot be added to a multiparty session.
//
#define UCC_E_SIP_PEER_PARTICIPANT_IN_MULTIPARTY_SESSION ((HRESULT)0x80EE0081L)

//
// MessageId: UCC_E_NOT_ALLOWED
//
// MessageText:
//
// This action is not allowed.
//
#define UCC_E_NOT_ALLOWED                ((HRESULT)0x80EE0082L)

//
// MessageId: UCC_E_REGISTRATION_DEACTIVATED
//
// MessageText:
//
// The user is being moved.
//
#define UCC_E_REGISTRATION_DEACTIVATED   ((HRESULT)0x80EE0083L)

//
// MessageId: UCC_E_REGISTRATION_REJECTED
//
// MessageText:
//
// The user's account was disabled or deleted or the SIP URI changed.
//
#define UCC_E_REGISTRATION_REJECTED      ((HRESULT)0x80EE0084L)

//
// MessageId: UCC_E_REGISTRATION_UNREGISTERED
//
// MessageText:
//
// The user was logged out because the user logged in elsewhere.
//
#define UCC_E_REGISTRATION_UNREGISTERED  ((HRESULT)0x80EE0085L)

//
// MessageId: UCC_E_BAD_RLMI_DOCUMENT
//
// MessageText:
//
// The RLMI document is invalid.
//
#define UCC_E_BAD_RLMI_DOCUMENT          ((HRESULT)0x80EE0086L)

//
// MessageId: UCC_E_INVALID_ID
//
// MessageText:
//
// The device ID is invalid.
//
#define UCC_E_INVALID_ID                 ((HRESULT)0x80EE0087L)

//
// MessageId: UCC_E_TRANSIENT_SERVER_DISCONNECT
//
// MessageText:
//
// This operation failed because the server is unreachable or busy at this time. Please try again later.
//
#define UCC_E_TRANSIENT_SERVER_DISCONNECT ((HRESULT)0x80EE0088L)

//
// MessageId: UCC_E_SIP_AUTHENTICATION_INCORRECT_REALM
//
// MessageText:
//
// Authentication failed because realm value in incoming message does not match realm value stored in endpoint.
//
#define UCC_E_SIP_AUTHENTICATION_INCORRECT_REALM ((HRESULT)0x80EE0089L)

//
// MessageId: UCC_E_ENDPOINT_DUPLICATE_USER_URI_AND_SERVER
//
// MessageText:
//
// An endpoint with the same User URI and SIP Server is already enabled.
//
#define UCC_E_ENDPOINT_DUPLICATE_USER_URI_AND_SERVER ((HRESULT)0x80EE008AL)

//
// MessageId: UCC_E_CANNOT_ADD_SPECIFIC_DEVICE_TO_SESSION
//
// MessageText:
//
// Cannot add a specific device to a session.
//
#define UCC_E_CANNOT_ADD_SPECIFIC_DEVICE_TO_SESSION ((HRESULT)0x80EE008BL)

//
// MessageId: UCC_E_INVALID_TEL_URL
//
// MessageText:
//
// The TEL URL is not valid.
//
#define UCC_E_INVALID_TEL_URL            ((HRESULT)0x80EE008CL)

//
// MessageId: UCC_E_PHONE_CONTROL_CHANNEL_NOT_EXISTS
//
// MessageText:
//
// The phone control channel does not exist.
//
#define UCC_E_PHONE_CONTROL_CHANNEL_NOT_EXISTS ((HRESULT)0x80EE008DL)

//
// MessageId: UCC_E_3RDPARTY_PHONE_SESSION_EXPECTED
//
// MessageText:
//
// 3rd-party phone control session expected.
//
#define UCC_E_3RDPARTY_PHONE_SESSION_EXPECTED ((HRESULT)0x80EE008EL)

//
// MessageId: UCC_E_DUPLICATE_3RDPARTY_PHONE_SESSION
//
// MessageText:
//
// 3rd-party phone control session with the same call-ID exists.
//
#define UCC_E_DUPLICATE_3RDPARTY_PHONE_SESSION ((HRESULT)0x80EE008FL)

//
// MessageId: UCC_E_CONSULTATION_CALL_IN_PROGRESS
//
// MessageText:
//
// Consultation call is already in progress.
//
#define UCC_E_CONSULTATION_CALL_IN_PROGRESS ((HRESULT)0x80EE0090L)

//
// MessageId: UCC_E_NOT_A_CONSULTATION_CALL
//
// MessageText:
//
// Operation is allowed only on consultation call which is not the case here.
//
#define UCC_E_NOT_A_CONSULTATION_CALL    ((HRESULT)0x80EE0091L)

//
// MessageId: UCC_E_INVALID_OPERATION
//
// MessageText:
//
// Performed operation is neither valid nor allowed.
//
#define UCC_E_INVALID_OPERATION          ((HRESULT)0x80EE0092L)

//
// MessageId: UCC_E_RESOURCE_UNAVAILABLE
//
// MessageText:
//
// A required resource is not available.
//
#define UCC_E_RESOURCE_UNAVAILABLE       ((HRESULT)0x80EE0093L)

//
// MessageId: UCC_E_INCOMPATIBLE_STATE
//
// MessageText:
//
// Performed operation is not compatible with current object state.
//
#define UCC_E_INCOMPATIBLE_STATE         ((HRESULT)0x80EE0094L)

//
// MessageId: UCC_E_INSUFFICIENT_SECURITY_LEVEL
//
// MessageText:
//
// Session was rejected due to insufficient security level.
//
#define UCC_E_INSUFFICIENT_SECURITY_LEVEL ((HRESULT)0x80EE0095L)

//
// MessageId: UCC_S_TRANSFER_SUCCESS
//
// MessageText:
//
// Transfer operation successful.
//
#define UCC_S_TRANSFER_SUCCESS           ((HRESULT)0x00EE0096L)

//
// MessageId: UCC_S_CONFERENCE_SUCCESS
//
// MessageText:
//
// Conference operation successful.
//
#define UCC_S_CONFERENCE_SUCCESS         ((HRESULT)0x00EE0097L)

//
// MessageId: UCC_S_CONFERENCE_JOIN_SUCCESS
//
// MessageText:
//
// Conference-join operation successful.
//
#define UCC_S_CONFERENCE_JOIN_SUCCESS    ((HRESULT)0x00EE0098L)

//
// MessageId: UCC_S_REMOTE_DISCONNECT_SUCCESS
//
// MessageText:
//
// Remote disconnect operation successful.
//
#define UCC_S_REMOTE_DISCONNECT_SUCCESS  ((HRESULT)0x00EE0099L)

//
// MessageId: UCC_S_DEFLECT_SUCCESS
//
// MessageText:
//
// Remote disconnect operation successful.
//
#define UCC_S_DEFLECT_SUCCESS            ((HRESULT)0x00EE009AL)

//
// MessageId: UCC_S_FORWARD_DISCONNECT_SUCCESS
//
// MessageText:
//
// Disconnected due to forwarding operation.
//
#define UCC_S_FORWARD_DISCONNECT_SUCCESS ((HRESULT)0x00EE009BL)

//
// MessageId: UCC_E_T120_NM_CONNECTION_FAILED
//
// MessageText:
//
// T120 NetMeeting connection failed.
//
#define UCC_E_T120_NM_CONNECTION_FAILED  ((HRESULT)0x80EE009CL)

//
// MessageId: UCC_E_NM_ALREADY_RUNNING
//
// MessageText:
//
// NetMeeting is already running.
//
#define UCC_E_NM_ALREADY_RUNNING         ((HRESULT)0x80EE009DL)

//
// MessageId: UCC_E_RESOURCE_BLOCKED
//
// MessageText:
//
// A required resource is blocked.
//
#define UCC_E_RESOURCE_BLOCKED           ((HRESULT)0x80EE009EL)

//
// MessageId: UCC_E_NOT_A_PRINCIPAL_CALL
//
// MessageText:
//
// Operation is allowed only on a principal call which is not the case here.
//
#define UCC_E_NOT_A_PRINCIPAL_CALL       ((HRESULT)0x80EE009FL)

//
// MessageId: UCC_E_PHONE_SERVER_INCAPABLE
//
// MessageText:
//
// The 3PCC server does not support basic operations.
//
#define UCC_E_PHONE_SERVER_INCAPABLE     ((HRESULT)0x80EE00A0L)

//
// MessageId: UCC_E_CONF_UNEXPECTED_FAILURE
//
// MessageText:
//
// Unexpected failure occured in the conference session.
//
#define UCC_E_CONF_UNEXPECTED_FAILURE    ((HRESULT)0x80EE00A1L)

//
// MessageId: UCC_E_NM_INITIALIZATION_FAIL
//
// MessageText:
//
// NetMeeting initialization failed.
//
#define UCC_E_NM_INITIALIZATION_FAIL     ((HRESULT)0x80EE00A2L)

//
// MessageId: UCC_E_INVALID_SERVER_VERSION
//
// MessageText:
//
// Server version received from the server is invalid.
//
#define UCC_E_INVALID_SERVER_VERSION     ((HRESULT)0x80EE00A3L)

//
// MessageId: UCC_S_RETRIEVE_SUCCESS
//
// MessageText:
//
// Server version received from the server is invalid.
//
#define UCC_S_RETRIEVE_SUCCESS           ((HRESULT)0x00EE00A4L)

//
// MessageId: UCC_E_MEDIA_TYPE_NOT_ALLOWED
//
// MessageText:
//
// Remote party does not support this media type.
//
#define UCC_E_MEDIA_TYPE_NOT_ALLOWED     ((HRESULT)0x80EE00A5L)

//
// MessageId: UCC_E_AUTHENTICATION_SERVER_UNAVAILABLE
//
// MessageText:
//
// Authenticating server cannot be reached.
//
#define UCC_E_AUTHENTICATION_SERVER_UNAVAILABLE ((HRESULT)0x80EE00A6L)

//
// MessageId: UCC_E_DIFFERENT_TRANSPORT_TO_SESSION
//
// MessageText:
//
// All participants in a session must have the same transport. Cannot add a participant with a different transport to a session.
//
#define UCC_E_DIFFERENT_TRANSPORT_TO_SESSION ((HRESULT)0x80EE00A7L)

//
// MessageId: UCC_S_UNSUBSCRIBED_BY_USER
//
// MessageText:
//
// This subscription has been terminated due to user action.
//
#define UCC_S_UNSUBSCRIBED_BY_USER       ((HRESULT)0x00EE00A8L)

//
// MessageId: UCC_E_TERMINATED_BY_SERVER
//
// MessageText:
//
// The subscription was unexpectedly terminated by the server.
//
#define UCC_E_TERMINATED_BY_SERVER       ((HRESULT)0x80EE00A9L)

//
// MessageId: UCC_E_IP_ADDRESS_CHANGED
//
// MessageText:
//
// Client IP address has changed. New IP address may not be immediately available.
//
#define UCC_E_IP_ADDRESS_CHANGED         ((HRESULT)0x80EE00AAL)

//
// MessageId: UCC_E_OUT_OF_ORDER_NOTIFY
//
// MessageText:
//
// Received an out of order notify message from server.
//
#define UCC_E_OUT_OF_ORDER_NOTIFY        ((HRESULT)0x80EE00ABL)

//
// MessageId: UCC_E_MALFORMED_HEADER_VALUE
//
// MessageText:
//
// Header value supplied was malformed.
//
#define UCC_E_MALFORMED_HEADER_VALUE     ((HRESULT)0x80EE00ACL)

//
// MessageId: UCC_E_MEDIA_ALLOCATION_FAILED
//
// MessageText:
//
// Media port allocation failed.
//
#define UCC_E_MEDIA_ALLOCATION_FAILED    ((HRESULT)0x80EE00ADL)

//
// MessageId: UCC_E_REQUEST_CANCELLED
//
// MessageText:
//
// Request was cancelled.
//
#define UCC_E_REQUEST_CANCELLED          ((HRESULT)0x80EE00AEL)

//
// MessageId: UCC_E_CONFERENCE_NOT_EXIST
//
// MessageText:
//
// Conference doesn't exist.
//
#define UCC_E_CONFERENCE_NOT_EXIST       ((HRESULT)0x80EE00AFL)

//
// MessageId: UCC_E_UNKNOWN_MCU_TYPE
//
// MessageText:
//
// Unknown MCU type.
//
#define UCC_E_UNKNOWN_MCU_TYPE           ((HRESULT)0x80EE00B0L)

//
// MessageId: UCC_E_ANONYMOUS_USERS_NOT_ALLOWED
//
// MessageText:
//
// Anonymous users not allowed.
//
#define UCC_E_ANONYMOUS_USERS_NOT_ALLOWED ((HRESULT)0x80EE00B1L)

//
// MessageId: UCC_E_CONFERENCE_EXISTS_ALREADY
//
// MessageText:
//
// Conference already exists. Possible duplicate conference ID.
//
#define UCC_E_CONFERENCE_EXISTS_ALREADY  ((HRESULT)0x80EE00B2L)

//
// MessageId: UCC_E_FEDERATED_USERS_NOT_ALLOWED
//
// MessageText:
//
// Federated users not allowed.
//
#define UCC_E_FEDERATED_USERS_NOT_ALLOWED ((HRESULT)0x80EE00B3L)

//
// MessageId: UCC_E_INVALID_ADMISSION_TYPE
//
// MessageText:
//
// Invalid conference admission type.
//
#define UCC_E_INVALID_ADMISSION_TYPE     ((HRESULT)0x80EE00B4L)

//
// MessageId: UCC_E_INVALID_ENCRYPTION_KEY
//
// MessageText:
//
// Invalid encryption key.
//
#define UCC_E_INVALID_ENCRYPTION_KEY     ((HRESULT)0x80EE00B5L)

//
// MessageId: UCC_E_INVALID_EXPIRY_TIME
//
// MessageText:
//
// Invalid conference expiry time.
//
#define UCC_E_INVALID_EXPIRY_TIME        ((HRESULT)0x80EE00B6L)

//
// MessageId: UCC_E_MCU_TYPE_NOT_AVAILABLE
//
// MessageText:
//
// MCU type not available.
//
#define UCC_E_MCU_TYPE_NOT_AVAILABLE     ((HRESULT)0x80EE00B7L)

//
// MessageId: UCC_E_MAX_CONFERENCES_EXCEEDED
//
// MessageText:
//
// Maximum number of conferences exceeded.
//
#define UCC_E_MAX_CONFERENCES_EXCEEDED   ((HRESULT)0x80EE00B8L)

//
// MessageId: UCC_E_MAX_MEETING_SIZE_EXCEEDED
//
// MessageText:
//
// Maximum meeting size exceeded.
//
#define UCC_E_MAX_MEETING_SIZE_EXCEEDED  ((HRESULT)0x80EE00B9L)

//
// MessageId: UCC_E_NOTIFICATION_DATA_TOO_LARGE
//
// MessageText:
//
// Conference participant notification data too large.
//
#define UCC_E_NOTIFICATION_DATA_TOO_LARGE ((HRESULT)0x80EE00BAL)

//
// MessageId: UCC_E_ORGANIZER_ROAMING_DATA_TOO_LARGE
//
// MessageText:
//
// Conference organizer roaming data too large.
//
#define UCC_E_ORGANIZER_ROAMING_DATA_TOO_LARGE ((HRESULT)0x80EE00BBL)

//
// MessageId: UCC_E_MEDIA_NOT_SUPPORTED
//
// MessageText:
//
// Media not supported.
//
#define UCC_E_MEDIA_NOT_SUPPORTED        ((HRESULT)0x80EE00BCL)

//
// MessageId: UCC_E_TRANSIENT_LOCAL_DISCONNECT
//
// MessageText:
//
// This operation failed because the network is unavailable at this time. Please try again later.
//
#define UCC_E_TRANSIENT_LOCAL_DISCONNECT ((HRESULT)0x80EE00BDL)

//
// MessageId: UCC_S_OPERATION_IN_PROGRESS
//
// MessageText:
//
// The operation is in progress.
//
#define UCC_S_OPERATION_IN_PROGRESS      ((HRESULT)0x00EE00BEL)

//
// MessageId: UCC_E_INVALID_SERVER_NAME
//
// MessageText:
//
// The specified server name is invalid.
//
#define UCC_E_INVALID_SERVER_NAME        ((HRESULT)0x80EE00BFL)

//
// MessageId: UCC_E_ALREADY_EXIST
//
// MessageText:
//
// The specified item already exists.
//
#define UCC_E_ALREADY_EXIST              ((HRESULT)0x80EE00C0L)

//
// MessageId: UCC_E_USER_NOT_ANSWERED
//
// MessageText:
//
// The user didn't answer the call.
//
#define UCC_E_USER_NOT_ANSWERED          ((HRESULT)0x80EE00C1L)

//
// MessageId: UCC_E_ANOTHER_RECORDING_IN_PROGRESS
//
// MessageText:
//
// Another recording already in progress.
//
#define UCC_E_ANOTHER_RECORDING_IN_PROGRESS ((HRESULT)0x80EE00C2L)

//
// MessageId: UCC_E_RECORDING_ALREADY_PAUSED
//
// MessageText:
//
// Recording already paused.
//
#define UCC_E_RECORDING_ALREADY_PAUSED   ((HRESULT)0x80EE00C3L)

//
// MessageId: UCC_E_RECORDING_NOT_PAUSED
//
// MessageText:
//
// Recording not paused.
//
#define UCC_E_RECORDING_NOT_PAUSED       ((HRESULT)0x80EE00C4L)

//
// MessageId: UCC_E_INVALID_SIP_DISPLAY_NAME
//
// MessageText:
//
// The SIP display name contains invalid characters.
//
#define UCC_E_INVALID_SIP_DISPLAY_NAME   ((HRESULT)0x80EE00C5L)

// Error codes from SIP status codes
//
// MessageId: UCC_S_SIP_STATUS_INFO_TRYING
//
// MessageText:
//
// SIP status code: 100 Trying.
//
#define UCC_S_SIP_STATUS_INFO_TRYING     ((HRESULT)0x00EF0064L)

//
// MessageId: UCC_S_SIP_STATUS_INFO_RINGING
//
// MessageText:
//
// SIP status code: 180 Ringing.
//
#define UCC_S_SIP_STATUS_INFO_RINGING    ((HRESULT)0x00EF00B4L)

//
// MessageId: UCC_S_SIP_STATUS_INFO_CALL_FORWARDING
//
// MessageText:
//
// SIP status code: 181 Call Is Being Forwarded.
//
#define UCC_S_SIP_STATUS_INFO_CALL_FORWARDING ((HRESULT)0x00EF00B5L)

//
// MessageId: UCC_S_SIP_STATUS_INFO_QUEUED
//
// MessageText:
//
// SIP status code: 182 Queued
//
#define UCC_S_SIP_STATUS_INFO_QUEUED     ((HRESULT)0x00EF00B6L)

//
// MessageId: UCC_S_SIP_STATUS_SESSION_PROGRESS
//
// MessageText:
//
// SIP status code: 183 Session Progress.
//
#define UCC_S_SIP_STATUS_SESSION_PROGRESS ((HRESULT)0x00EF00B7L)

//
// MessageId: UCC_S_SIP_STATUS_SUCCESS
//
// MessageText:
//
// SIP status code: 200 OK.
//
#define UCC_S_SIP_STATUS_SUCCESS         ((HRESULT)0x00EF00C8L)

//
// MessageId: UCC_S_SIP_STATUS_ACCEPTED
//
// MessageText:
//
// SIP status code: 202 Accepted.
//
#define UCC_S_SIP_STATUS_ACCEPTED        ((HRESULT)0x00EF00CAL)

//
// MessageId: UCC_E_SIP_STATUS_REDIRECT_MULTIPLE_CHOICES
//
// MessageText:
//
// SIP status code: 300 Multiple Choices.
//
#define UCC_E_SIP_STATUS_REDIRECT_MULTIPLE_CHOICES ((HRESULT)0x80EF012CL)

//
// MessageId: UCC_E_SIP_STATUS_REDIRECT_MOVED_PERMANENTLY
//
// MessageText:
//
// SIP status code: 301 Moved Permanently.
//
#define UCC_E_SIP_STATUS_REDIRECT_MOVED_PERMANENTLY ((HRESULT)0x80EF012DL)

//
// MessageId: UCC_E_SIP_STATUS_REDIRECT_MOVED_TEMPORARILY
//
// MessageText:
//
// SIP status code: 302 Moved Temporarily.
//
#define UCC_E_SIP_STATUS_REDIRECT_MOVED_TEMPORARILY ((HRESULT)0x80EF012EL)

//
// MessageId: UCC_E_SIP_STATUS_REDIRECT_SEE_OTHER
//
// MessageText:
//
// SIP status code: 303 See Other.
//
#define UCC_E_SIP_STATUS_REDIRECT_SEE_OTHER ((HRESULT)0x80EF012FL)

//
// MessageId: UCC_E_SIP_STATUS_REDIRECT_USE_PROXY
//
// MessageText:
//
// SIP status code: 305 Use Proxy.
//
#define UCC_E_SIP_STATUS_REDIRECT_USE_PROXY ((HRESULT)0x80EF0131L)

//
// MessageId: UCC_E_SIP_STATUS_REDIRECT_ALTERNATIVE_SERVICE
//
// MessageText:
//
// SIP status code: 380 Alternative Service.
//
#define UCC_E_SIP_STATUS_REDIRECT_ALTERNATIVE_SERVICE ((HRESULT)0x80EF017CL)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_BAD_REQUEST
//
// MessageText:
//
// SIP status code: 400 Bad Request.
//
#define UCC_E_SIP_STATUS_CLIENT_BAD_REQUEST ((HRESULT)0x80EF0190L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_UNAUTHORIZED
//
// MessageText:
//
// SIP status code: 401 Unauthorized.
//
#define UCC_E_SIP_STATUS_CLIENT_UNAUTHORIZED ((HRESULT)0x80EF0191L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_PAYMENT_REQUIRED
//
// MessageText:
//
// SIP status code: 402 Payment Required.
//
#define UCC_E_SIP_STATUS_CLIENT_PAYMENT_REQUIRED ((HRESULT)0x80EF0192L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_FORBIDDEN
//
// MessageText:
//
// SIP status code: 403 Forbidden.
//
#define UCC_E_SIP_STATUS_CLIENT_FORBIDDEN ((HRESULT)0x80EF0193L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_NOT_FOUND
//
// MessageText:
//
// SIP status code: 404 Not Found.
//
#define UCC_E_SIP_STATUS_CLIENT_NOT_FOUND ((HRESULT)0x80EF0194L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_METHOD_NOT_ALLOWED
//
// MessageText:
//
// SIP status code: 405 Method Not Allowed.
//
#define UCC_E_SIP_STATUS_CLIENT_METHOD_NOT_ALLOWED ((HRESULT)0x80EF0195L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_NOT_ACCEPTABLE
//
// MessageText:
//
// SIP status code: 406 Not Acceptable.
//
#define UCC_E_SIP_STATUS_CLIENT_NOT_ACCEPTABLE ((HRESULT)0x80EF0196L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_PROXY_AUTHENTICATION_REQUIRED
//
// MessageText:
//
// SIP status code: 407 Proxy Authentication Required.
//
#define UCC_E_SIP_STATUS_CLIENT_PROXY_AUTHENTICATION_REQUIRED ((HRESULT)0x80EF0197L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_REQUEST_TIMEOUT
//
// MessageText:
//
// SIP status code: 408 Request Timeout.
//
#define UCC_E_SIP_STATUS_CLIENT_REQUEST_TIMEOUT ((HRESULT)0x80EF0198L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_CONFLICT
//
// MessageText:
//
// SIP status code: 409 Conflict.
//
#define UCC_E_SIP_STATUS_CLIENT_CONFLICT ((HRESULT)0x80EF0199L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_GONE
//
// MessageText:
//
// SIP status code: 410 Gone.
//
#define UCC_E_SIP_STATUS_CLIENT_GONE     ((HRESULT)0x80EF019AL)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_LENGTH_REQUIRED
//
// MessageText:
//
// SIP status code: 411 Length Required.
//
#define UCC_E_SIP_STATUS_CLIENT_LENGTH_REQUIRED ((HRESULT)0x80EF019BL)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_REQUEST_ENTITY_TOO_LARGE
//
// MessageText:
//
// SIP status code: 413 Request Entity Too Large.
//
#define UCC_E_SIP_STATUS_CLIENT_REQUEST_ENTITY_TOO_LARGE ((HRESULT)0x80EF019DL)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_REQUEST_URI_TOO_LARGE
//
// MessageText:
//
// SIP status code: 414 Request-URI Too Long.
//
#define UCC_E_SIP_STATUS_CLIENT_REQUEST_URI_TOO_LARGE ((HRESULT)0x80EF019EL)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_UNSUPPORTED_MEDIA_TYPE
//
// MessageText:
//
// SIP status code: 415 Unsupported Media Type.
//
#define UCC_E_SIP_STATUS_CLIENT_UNSUPPORTED_MEDIA_TYPE ((HRESULT)0x80EF019FL)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_BAD_EXTENSION
//
// MessageText:
//
// SIP status code: 420 Bad Extension.
//
#define UCC_E_SIP_STATUS_CLIENT_BAD_EXTENSION ((HRESULT)0x80EF01A4L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_EXTENSION_REQUIRED
//
// MessageText:
//
// SIP status code: 421 Extension Required.
//
#define UCC_E_SIP_STATUS_CLIENT_EXTENSION_REQUIRED ((HRESULT)0x80EF01A5L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_TEMPORARILY_NOT_AVAILABLE
//
// MessageText:
//
// SIP status code: 480 Temporarily Unavailable.
//
#define UCC_E_SIP_STATUS_CLIENT_TEMPORARILY_NOT_AVAILABLE ((HRESULT)0x80EF01E0L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_TRANSACTION_DOES_NOT_EXIST
//
// MessageText:
//
// SIP status code: 481 Call Leg/Transaction Does Not Exist.
//
#define UCC_E_SIP_STATUS_CLIENT_TRANSACTION_DOES_NOT_EXIST ((HRESULT)0x80EF01E1L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_LOOP_DETECTED
//
// MessageText:
//
// SIP status code: 482 Loop Detected.
//
#define UCC_E_SIP_STATUS_CLIENT_LOOP_DETECTED ((HRESULT)0x80EF01E2L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_TOO_MANY_HOPS
//
// MessageText:
//
// SIP status code: 483 Too Many Hops.
//
#define UCC_E_SIP_STATUS_CLIENT_TOO_MANY_HOPS ((HRESULT)0x80EF01E3L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_ADDRESS_INCOMPLETE
//
// MessageText:
//
// SIP status code: 484 Address Incomplete.
//
#define UCC_E_SIP_STATUS_CLIENT_ADDRESS_INCOMPLETE ((HRESULT)0x80EF01E4L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_AMBIGUOUS
//
// MessageText:
//
// SIP status code: 485 Ambiguous.
//
#define UCC_E_SIP_STATUS_CLIENT_AMBIGUOUS ((HRESULT)0x80EF01E5L)

//
// MessageId: UCC_E_SIP_STATUS_CLIENT_BUSY_HERE
//
// MessageText:
//
// SIP status code: 486 Busy Here.
//
#define UCC_E_SIP_STATUS_CLIENT_BUSY_HERE ((HRESULT)0x80EF01E6L)

//
// MessageId: UCC_E_SIP_STATUS_REQUEST_TERMINATED
//
// MessageText:
//
// SIP status code: 487 Request Terminated.
//
#define UCC_E_SIP_STATUS_REQUEST_TERMINATED ((HRESULT)0x80EF01E7L)

//
// MessageId: UCC_S_SIP_STATUS_REQUEST_TERMINATED
//
// MessageText:
//
// SIP status code: 487 Request Terminated.
//
#define UCC_S_SIP_STATUS_REQUEST_TERMINATED ((HRESULT)0x00EF01E7L)

//
// MessageId: UCC_E_SIP_STATUS_NOT_ACCEPTABLE_HERE
//
// MessageText:
//
// SIP status code: 488 Not Acceptable Here.
//
#define UCC_E_SIP_STATUS_NOT_ACCEPTABLE_HERE ((HRESULT)0x80EF01E8L)

//
// MessageId: UCC_E_SIP_STATUS_REQUEST_PENDING
//
// MessageText:
//
// SIP status code: 491 Request Pending.
//
#define UCC_E_SIP_STATUS_REQUEST_PENDING ((HRESULT)0x80EF01EBL)

//
// MessageId: UCC_E_SIP_STATUS_SERVER_INTERNAL_ERROR
//
// MessageText:
//
// SIP status code: 500 Server Internal Error.
//
#define UCC_E_SIP_STATUS_SERVER_INTERNAL_ERROR ((HRESULT)0x80EF01F4L)

//
// MessageId: UCC_E_SIP_STATUS_SERVER_NOT_IMPLEMENTED
//
// MessageText:
//
// SIP status code: 501 Not Implemented.
//
#define UCC_E_SIP_STATUS_SERVER_NOT_IMPLEMENTED ((HRESULT)0x80EF01F5L)

//
// MessageId: UCC_E_SIP_STATUS_SERVER_BAD_GATEWAY
//
// MessageText:
//
// SIP status code: 502 Bad Gateway.
//
#define UCC_E_SIP_STATUS_SERVER_BAD_GATEWAY ((HRESULT)0x80EF01F6L)

//
// MessageId: UCC_E_SIP_STATUS_SERVER_SERVICE_UNAVAILABLE
//
// MessageText:
//
// SIP status code: 503 Service Unavailable.
//
#define UCC_E_SIP_STATUS_SERVER_SERVICE_UNAVAILABLE ((HRESULT)0x80EF01F7L)

//
// MessageId: UCC_E_SIP_STATUS_SERVER_SERVER_TIMEOUT
//
// MessageText:
//
// SIP status code: 504 Server Time-out.
//
#define UCC_E_SIP_STATUS_SERVER_SERVER_TIMEOUT ((HRESULT)0x80EF01F8L)

//
// MessageId: UCC_E_SIP_STATUS_SERVER_VERSION_NOT_SUPPORTED
//
// MessageText:
//
// SIP status code: 505 Version Not Supported.
//
#define UCC_E_SIP_STATUS_SERVER_VERSION_NOT_SUPPORTED ((HRESULT)0x80EF01F9L)

//
// MessageId: UCC_E_SIP_STATUS_MESSAGE_TOO_LARGE
//
// MessageText:
//
// SIP status code: 513 Message Too Large.
//
#define UCC_E_SIP_STATUS_MESSAGE_TOO_LARGE ((HRESULT)0x80EF0201L)

//
// MessageId: UCC_E_SIP_STATUS_GLOBAL_BUSY_EVERYWHERE
//
// MessageText:
//
// SIP status code: 600 Busy Everywhere.
//
#define UCC_E_SIP_STATUS_GLOBAL_BUSY_EVERYWHERE ((HRESULT)0x80EF0258L)

//
// MessageId: UCC_E_SIP_STATUS_GLOBAL_DECLINE
//
// MessageText:
//
// SIP status code: 603 Decline.
//
#define UCC_E_SIP_STATUS_GLOBAL_DECLINE  ((HRESULT)0x80EF025BL)

//
// MessageId: UCC_E_SIP_STATUS_GLOBAL_DOES_NOT_EXIST_ANYWHERE
//
// MessageText:
//
// SIP status code: 604 Does Not Exist Anywhere.
//
#define UCC_E_SIP_STATUS_GLOBAL_DOES_NOT_EXIST_ANYWHERE ((HRESULT)0x80EF025CL)

//
// MessageId: UCC_E_SIP_STATUS_GLOBAL_DECLINE_EVERYWHERE
//
// MessageText:
//
// SIP status code: 605 Decline Everywhere.
//
#define UCC_E_SIP_STATUS_GLOBAL_DECLINE_EVERYWHERE ((HRESULT)0x80EF025DL)

//
// MessageId: UCC_E_SIP_STATUS_GLOBAL_NOT_ACCEPTABLE
//
// MessageText:
//
// SIP status code: 606 Not Acceptable.
//
#define UCC_E_SIP_STATUS_GLOBAL_NOT_ACCEPTABLE ((HRESULT)0x80EF025EL)

//
// MessageId: UCC_E_SIP_STATUS_NOT_IN_BATCH
//
// MessageText:
//
// SIP status code: 607 Not In Batch.
//
#define UCC_E_SIP_STATUS_NOT_IN_BATCH    ((HRESULT)0x80EF025FL)

//
// MessageId: UCC_E_SIP_STATUS_NOT_SUPPORTED
//
// MessageText:
//
// SIP status code: 608 Not Supported.
//
#define UCC_E_SIP_STATUS_NOT_SUPPORTED   ((HRESULT)0x80EF0260L)

//
// MessageId: UCC_E_SIP_STATUS_NO_DATA
//
// MessageText:
//
// SIP status code: 609 No Data.
//
#define UCC_E_SIP_STATUS_NO_DATA         ((HRESULT)0x80EF0261L)

//
// MessageId: UCC_E_SIP_STATUS_NOT_MEMBER
//
// MessageText:
//
// SIP status code: 610 Not Member.
//
#define UCC_E_SIP_STATUS_NOT_MEMBER      ((HRESULT)0x80EF0262L)

//
// MessageId: UCC_E_SIP_STATUS_RESTART_SESSION
//
// MessageText:
//
// SIP status code: 611 Restart Session.
//
#define UCC_E_SIP_STATUS_RESTART_SESSION ((HRESULT)0x80EF0263L)

//
// MessageId: UCC_E_SIP_STATUS_PENDING
//
// MessageText:
//
// SIP status code: 612 Pending.
//
#define UCC_E_SIP_STATUS_PENDING         ((HRESULT)0x80EF0264L)

