import { StyleSheet, Dimensions } from "react-native";

export const styles = StyleSheet.create({
    //Universal
    flexView: { flex: 1 },
    zIndex: { zIndex: 100 },
    justifyAlign: { justifyContent: "center", alignItems: "center" },
    alignCenter: { alignItems: "center" },
    marginTopXs: { marginTop: 2 },
    marginTopM: { marginTop: 15 },
    marginRightS: { marginRight: 5 },
    marginVerticalM: { marginVertical: 10 },
    marginVerticalL: { marginVertical: 15 },
    paddingBottomS: { paddingBottom: 5 },
    flexColumn: { flexDirection: "column" },
    flexColumnStart: { flexDirection: "column", justifyContent: "flex-start" },
    flexRow: { flexDirection: "row" },
    flexRowStart: { flexDirection: "row", justifyContent: "flex-start" },
    flexRowCenter: { flexDirection: "row", alignItems: "center" },
    flexRowSpace: {
        flexDirection: "row",
        alignItems: "center",
        justifyContent: "space-between",
    },

    //Render list
    positionTouchable: {
        alignItems: "center",
        paddingVertical: 4,
    },
    padRender: { paddingHorizontal: 10, paddingVertical: 4 },
    paddingList: {
        paddingHorizontal: 10,
        borderRadius: 10,
        margin: 4,
        backgroundColor: "#fff",
        alignSelf: "center",
    },
    shadowRender: {
        borderRadius: 10,
        padding: 10,
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.5,
        shadowRadius: 2,
        elevation: 3,
    },
    touchCarData: {
        paddingVertical: 5,
        borderRadius: 20,
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.6,
        shadowRadius: 2,
        elevation: 3,
    },
    dataCar: {
        fontSize: 26,
        fontWeight: "bold",
        paddingRight: 5,
    },
    //change
    fontRFID: { fontSize: 15 },
    backSuffix: {
        position: "absolute",
        left: 0,
        right: 0,
        bottom: 0,
        top: 40,
    },
    // change
    column: {
        flexDirection: "column",
        justifyContent: "space-between",
        marginLeft: 5,
        flex: 1,
    },

    //Search bar
    searchBar: {
        flexDirection: "row",
        alignItems: "center",
        marginHorizontal: 10,
        borderRadius: 50,
        padding: 10,
        marginTop: 10,
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.6,
        shadowRadius: 2.0,
        elevation: 3,
        position: "absolute",
    },
    searchBarIcon: { borderRadius: 50, padding: 5, right: 1 },
    searchBarTextInput: { padding: 10, flex: 10, fontSize: 20 },

    //Error
    errStyle: {
        alignSelf: "center",
        marginTop: 150,
    },
    errText: {
        fontSize: 20,
        color: "#FFFFFF",
        padding: "5%",
        textAlign: "center",
        fontWeight: "800",
    },
    errView: {
        backgroundColor: "#D5262F",
        borderRadius: 10,
        borderWidth: 5,
        borderColor: "#FFF",
    },
    errImage: {
        height: "60%",
        width: undefined,
    },

    //Modal
    modal: {
        flex: 1,
        height: "100%",
    },
    modalContainer: {
        flex: 5,
        justifyContent: "flex-start",
        backgroundColor: "rgba(0, 0, 0, 0.6)",
    },
    viewSome: {
        flex: 1,
        justifyContent: "flex-end",
    },
    viewModal: {
        alignItems: "center",
        borderRadius: 6,
        width: "90%",
        shadowColor: "#000",
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.6,
        shadowRadius: 2,
        elevation: 3,
        marginVertical: 15,
    },
    viewInfo: {
        width: "93%",
        borderRadius: 5,
    },
    bodyModal: {
        alignItems: "center",
        width: "100%",
        borderTopRightRadius: 15,
        borderTopLeftRadius: 15,
    },
    viewWidth: {
        width: "90%",
        shadowColor: "#000",
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.6,
        shadowRadius: 2,
        elevation: 3,
    },
    plateModal: {
        marginVertical: "5%",
        // backgroundColor: "#3778B5",
        borderRadius: 5,
        elevation: 3,
        // #2067A9
    },
    backSpeed: {
        padding: 10,
        margin: 10,
        borderRadius: 4,
        shadowColor: "#000",
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.6,
        shadowRadius: 2,
        elevation: 0,
    },
    suffixModal: {
        position: "absolute",
        left: 0,
        right: 0,
        bottom: 0,
        top: 50,
    },
    suffixText: {
        color: "#000",
        fontSize: 13,
        textAlign: "center",
        fontWeight: "400",
    },
    justAlign: {
        justifyContent: "center",
        alignItems: "center",
        flex: 1,
    },

    plateInModal: {
        flexDirection: "row",
        justifyContent: "flex-start",
        alignItems: "center",
    },
    plateLicenseText: {
        fontSize: 30,
        fontWeight: "bold",
        color: "#FFF",
        textAlign: "center",
    },

    imageModal: {
        width: "100%",
        height: undefined,
        aspectRatio: 16 / 9,
        resizeMode: "cover",
        borderColor: "#F7F0F5",
        borderWidth: 2,
        borderRadius: 2,
    },
    imageBody: {
        margin: 10,
        shadowColor: "#000",
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.6,
        shadowRadius: 2,
        elevation: 3,
    },
    imageBackground: {
        position: "absolute",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        justifyContent: "center",
        alignItems: "center",
    },
    smallImage: {
        // width: "100%",
        // height: undefined,
        // aspectRatio: 2,
        // resizeMode: "contain",
        alignSelf: "center",
        height: 50,
        width: 200,
        resizeMode: "stretch",
    },
    smallImagePosition: {
        position: "absolute",
        zIndex: 100,
        left: 0,
        top: 0,
    },
    smallWarning: {
        backgroundColor: "rgba(181,100,104, 0.93)",
        flexDirection: "row",
        alignItems: "center",
        padding: 4,
    },
    bigWarning: {
        backgroundColor: "rgba(181,100,104, 0.93)",
        flexDirection: "row",
        alignItems: "center",
        paddingRight: 10,
    },
    smallWarningText: { fontSize: 9, fontWeight: "700", paddingLeft: 10 },
    bigWarningText: { fontSize: 19, fontWeight: "700", paddingLeft: 10 },

    shareButton: { position: "absolute", top: 10, right: 0, zIndex: 1 },

    detailsLayout: { flexDirection: "row", padding: 5 },
    detailsColumn: {
        flexDirection: "column",
        marginVertical: 10,
        marginRight: 4,
    },
    detailsSecondColumn: {
        flexDirection: "column",
        marginVertical: 10,
        marginLeft: 7,
        flex: 1,
    },

    subTitleModal: {
        fontSize: 15,
        color: "#F7F0F5",
    },

    rfidWarning: {
        flexDirection: "row",
        alignItems: "center",
        borderRadius: 3,
    },
    rfidWarningText: { fontSize: 15, fontWeight: "700", paddingHorizontal: 4 },

    touchClose: {
        paddingVertical: "3%",
        paddingHorizontal: "7%",
        backgroundColor: "#F7F0F5",
        borderRadius: 25,
        alignSelf: "center",
    },
    closeText: {
        color: "#2067A9",
        textAlign: "center",
        fontSize: 20,
        fontWeight: "bold",
    },

    //Edit Modal
    editTitle: {
        padding: 15,
        flexDirection: "row",
        justifyContent: "space-between",
        alignItems: "center",
    },
    editTitleText: { fontSize: 30, fontWeight: "bold", textAlign: "center" },
    buttonReset: { padding: 5, borderRadius: 3 },
    buttonSave: {
        padding: 5,
        borderRadius: 3,
        marginLeft: 5,
    },
    editColorTagContainer: {
        flexDirection: "row",
        flex: 1,
        marginTop: 10,
        justifyContent: "space-evenly",
        marginHorizontal: 10,
    },
    //Home
    viewHomeLocation: {
        width: "100%",
        alignItems: "center",
        flexDirection: "row",
        borderRadius: 10,
        shadowColor: "#000",
        shadowOffset: {
            width: 0,
            height: 1,
        },
        shadowOpacity: 0.18,
        shadowRadius: 1.0,
        elevation: 12,
    },
    region: {
        position: "absolute",
        bottom: 0,
        zIndex: 100,
        backgroundColor: "#13548A",
        width: "100%",
        flexDirection: "row",
        alignItems: "center",
        justifyContent: "center",
        padding: 5,
    },
    regionText: {
        color: "#FFF",
        fontSize: 13,
        padding: 3,
        textAlign: "center",
    },
    floatingButtonHome: {
        alignItems: "center",
        justifyContent: "center",
        borderRadius: 50,
        padding: 6,
        shadowColor: "#000",
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.5,
        shadowRadius: 2.0,
        elevation: 10,
    },
    locationText: {
        fontSize: 20,
        fontWeight: "bold",
        padding: 20,
    },
    locationCard: {
        borderTopRightRadius: 10,
        borderTopLeftRadius: 10,
        shadowOffset: {
            width: 0,
            height: 2,
        },
        shadowOpacity: 0.3,
        shadowRadius: 3.0,
        elevation: 30,
    },
    padHomeRender: { paddingVertical: 10, paddingHorizontal: 5 },
    searchBarHome: {
        flexDirection: "row",
        alignItems: "center",
        marginHorizontal: 3,
        borderRadius: 50,
        padding: 10,
        flex: 1,
        marginBottom: 5,
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.6,
        shadowRadius: 2.0,
        elevation: 3,
    },
    homeSearchBg: {
        flexDirection: "row",
        alignSelf: "center",
        marginTop: 10,
        zIndex: 100,
        position: "absolute",
    },
    map: {
        width: Dimensions.get("window").width,
        height: Dimensions.get("window").height,
    },
    mapConfig: {
        width: Dimensions.get("window").width * 0.9,
        height: Dimensions.get("window").height * 0.4,
    },

    calloutContainer: { borderRadius: 0, paddingRight: 10 },
    calloutFlex: { flex: 1, flexDirection: "row", alignItems: "center" },

    chevronIcon: {
        borderRadius: 50,
        padding: 5,
        top: -60,
        right: 10,
        position: "absolute",
    },

    //Watchlist
    floatingButton: {
        alignItems: "center",
        justifyContent: "center",
        width: 70,
        position: "absolute",
        bottom: 20,
        right: 20,
        height: 70,
        borderRadius: 100,
        zIndex: 1,
        shadowColor: "#000",
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.5,
        shadowRadius: 2.0,
        elevation: 3,
    },
    editButton: { zIndex: 100, marginLeft: 5 },

    tagWatchlist: {
        flexDirection: "row",
        alignItems: "center",
        paddingHorizontal: 0,
        borderRadius: 3,
    },
    tagCircle: { width: 10, height: 10, borderRadius: 50, marginRight: 1 },
    tagText: {
        fontWeight: "700",
        paddingHorizontal: 4,
        //fontSize: 14,
    },
    commentText: { fontSize: 14 },
    dateWatchlistText: { fontSize: 14 },
    commentBreak: { padding: 1, marginVertical: 4 },

    formContainer: {
        padding: 10,
        margin: 10,
        borderRadius: 10,
        shadowOffset: {
            width: 0,
            height: 0,
        },
        shadowOpacity: 0.5,
        shadowRadius: 2,
        elevation: 3,
    },
    formTag: { fontSize: 20, fontWeight: "bold" },
    submitFormMargin: { marginTop: 50, alignSelf: "center" },
    submitFormButton: { padding: 10, paddingHorizontal: 60, borderRadius: 10 },
    submitFormText: { fontSize: 20, fontWeight: "600" },

    //Settings
    settingsLogo: {
        width: "100%",
        height: undefined,
        aspectRatio: 1 / 1,
    },
    imageLogo: {
        justifyContent: "center",
        alignItems: "center",
        marginBottom: "5%",
    },

    //Tags
    chip: { flexDirection: "row", alignItems: "center", borderRadius: 50 },
    avatar: { width: 10, height: 10, borderRadius: 50, margin: 8 },
    chipText: { fontSize: 14, paddingRight: 9, fontWeight: "700" },

    //Details
    detailsContainer: {
        flexDirection: "row",
        justifyContent: "flex-start",
        justifyContent: "space-between",
    },
    detailsChip: {
        flexDirection: "row",
        alignItems: "center",
        flex: 1,
    },
    detailsChipPadding: {
        flexDirection: "row",
        alignItems: "center",
        paddingHorizontal: 2,
        borderRadius: 3,
    },
    detailsAvatar: { width: 10, height: 10, borderRadius: 50, margin: 1 },
    detailsChipText: {
        fontSize: 12,
        paddingHorizontal: 4,
        fontWeight: "700",
        flex: 1,
    },
    detailsNoRfid: {
        flexDirection: "row",
        alignItems: "center",
        borderRadius: 3,
    },
    detailsRfidText: { fontSize: 14, fontWeight: "700" },

    //Lottie
    lottieLoad: {
        flexDirection: "row",
        alignItems: "center",
        padding: 30,
    },
    lottieBox: {
        height: 400,
        padding: 10,
    },
    lottieLocation: {
        height: 400,
        padding: 20,
    },
});
