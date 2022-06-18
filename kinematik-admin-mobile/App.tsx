import React from "react";
import {
  StatusBar,
  StyleSheet,
  Text,
  TouchableOpacity,
  Dimensions,
  View,
  Platform,
  ViewStyle,
  StyleProp,
  Button,
} from "react-native";

import { BarCodeScanningResult, Camera } from "expo-camera";
import { BarCodeScanner } from "expo-barcode-scanner";
import * as NavigationBar from "expo-navigation-bar";

import { RootSiblingParent } from "react-native-root-siblings";

import ApiInteraction from "./api-interaction/api-interaction";
import { CheckTicketErrorCode } from "./api-interaction/responses/checkTicketResponse";
import Hall from "./domain/hall";

import ErrorBoundary from "react-native-error-boundary";

import Toast from "react-native-root-toast";
import ErrorFallbackComponent from "./error-handling/ErrorFallbackComponent/index";

interface AppState {
  isBarCodePermissionGranted: boolean;
  shouldScan: boolean;

  isLoadingHalls: boolean;
  availableHalls: Hall[];

  isCheckingTicket: boolean;
  checkedHallID: number | null;

  cameraRatio: string;
  isCameraRatioSet: boolean;
  cameraPadding: number;
  realHeight: number | null;
  realWidth: number | null;
}

const DEFAULT_CAMERA_RATIO: string = "4:3";
const DEFAULT_PALETTE = {
  button: {
    primary: {
      background: "rgb(226, 30, 32)",
      border: "rgb(222, 129, 131)",
      text: "rgb(255, 255, 255)",
    },
  },
  appBackgorund: "rgb(41, 37, 36)",
  navigationBarBackground: "rgba(41, 37, 36, 1)",
};

export default class App extends React.Component {
  camera: React.RefObject<Camera> | null = null;
  state: AppState = {
    isBarCodePermissionGranted: false,
    shouldScan: true,

    isLoadingHalls: true,
    availableHalls: [],

    isCheckingTicket: false,
    checkedHallID: null,

    cameraRatio: DEFAULT_CAMERA_RATIO,
    isCameraRatioSet: false,
    cameraPadding: 0,
    realHeight: null,
    realWidth: null,
  };

  constructor(props: never) {
    super(props);

    this.camera = React.createRef();
  }

  async componentDidMount() {
    StatusBar.setHidden(true);
    await NavigationBar.setBackgroundColorAsync("rgba(41, 37, 36, 1)");
    await this.getPermissionsAsync();

    await this.getAvailableHalls();
  }

  getPermissionsAsync = async () => {
    const { status } = await Camera.requestCameraPermissionsAsync();
    this.setState({
      isBarCodePermissionGranted: status === "granted",
    });
  };

  displayError = (errorMessage: string) => {
    Toast.show(errorMessage, {
      duration: 7500,
      position: Toast.positions.TOP,
      backgroundColor: "rgb(226, 30, 32)",
    });
  };

  render() {
    const barCodeScannerStyles: StyleProp<ViewStyle> = [
      styles.barCodeScanner,
      {
        top: this.state.cameraPadding / 2,
      },
    ];
    if (this.state.realHeight != null && this.state.realWidth != null) {
      barCodeScannerStyles.push({
        height: this.state.realHeight,
        width: this.state.realWidth,
      });
    }

    return (
      <RootSiblingParent>
        <ErrorBoundary FallbackComponent={ErrorFallbackComponent}>
          <View style={styles.appContainer}>
            <Camera
              barCodeScannerSettings={{
                barCodeTypes: [BarCodeScanner.Constants.BarCodeType.qr],
              }}
              onBarCodeScanned={
                this.state.shouldScan ? this.handleQrCodeScanned : undefined
              }
              onCameraReady={this.setNecessaryAspectRatio}
              ratio={this.state.cameraRatio}
              style={barCodeScannerStyles}
              ref={this.camera}
            ></Camera>

            <View style={styles.hallChoosingArea}>
              {this.state.availableHalls.map((availableHall) => {
                return (
                  <TouchableOpacity
                    onPress={() => this.changeCheckedHall(availableHall.id)}
                    style={styles.hallChoosingButton}
                    key={availableHall.id}
                    disabled={availableHall.id === this.state.checkedHallID}
                  >
                    <Text style={styles.hallChoosingButtonLabel}>
                      {availableHall.title}
                    </Text>
                  </TouchableOpacity>
                );
              })}
            </View>
          </View>
        </ErrorBoundary>
      </RootSiblingParent>
    );
  }

  getAvailableHalls = async () => {
    this.setState({
      isLoadingHalls: true,
    });

    const halls = await ApiInteraction.getAvailableHalls();

    this.setState({
      isLoadingHalls: false,
      availableHalls: halls,
      checkedHallID: halls.length > 0 ? halls[0].id : null,
    });
  };

  changeCheckedHall = async (hallID: number) => {
    this.setState({
      checkedHallID: hallID,
    });
  };

  handleQrCodeScanned = async (scanningResult: BarCodeScanningResult) => {
    if (this.state.checkedHallID !== null) {
      this.setState({
        shouldScan: false,
      });

      await this.checkTicket(Number.parseInt(scanningResult.data));

      this.setState({
        shouldScan: true,
      });
    }
  };

  checkTicket = async (bookingID: number) => {
    this.setState({
      isCheckingTicket: true,
    });

    const checkTicketResponse = await ApiInteraction.checkTicket(
      bookingID,
      this.state.checkedHallID!
    );

    if (checkTicketResponse.isValid) {
      alert("Квиток коректний.");
    } else {
      switch (checkTicketResponse.errorCode) {
        case CheckTicketErrorCode.NOT_EXISTING:
          alert("Неправильний QR-код!");
          break;
        case CheckTicketErrorCode.NOT_PAYED_FOR:
          alert("За квиток не заплачено!");
          break;
        case CheckTicketErrorCode.WRONG_HALL:
          alert("Неправильна зала!");
          break;
      }
    }

    this.setState({
      isCheckingTicket: false,
    });
  };

  setNecessaryAspectRatio = async () => {
    if (this.state.isCameraRatioSet) {
      return;
    }

    // Start with the system default
    let desiredRatio = DEFAULT_CAMERA_RATIO;
    // This issue only affects Android
    if (Platform.OS === "android") {
      const ratios = await this.camera!.current!.getSupportedRatiosAsync();

      // Calculate the width/height of each of the supported camera ratios
      // These width/height are measured in landscape mode
      // find the ratio that is closest to the screen ratio without going over
      let distances: { [index: string]: number } = {};
      let realRatios: { [index: string]: number } = {};
      let minDistance = null;

      const { height: screenHeight, width: screenWidth } =
        Dimensions.get("screen");
      const screenRatio = screenHeight / screenWidth;

      for (const ratio of ratios) {
        const parts = ratio.split(":");
        const realRatio = parseInt(parts[0]) / parseInt(parts[1]);
        realRatios[ratio] = realRatio;
        // ratio can't be taller than screen, so we don't want an abs()

        const distance = screenRatio - realRatio;
        distances[ratio] = realRatio;
        if (minDistance == null) {
          minDistance = ratio;
        } else {
          if (distance >= 0 && distance < distances[minDistance]) {
            minDistance = ratio;
          }
        }
      }

      // set the best match
      if (minDistance !== null) {
        desiredRatio = minDistance;
      }

      //  calculate the difference between the camera width and the screen height
      const realHeight = realRatios[desiredRatio] * screenWidth;
      const heightDifference = Math.floor((screenHeight - realHeight) / 2);

      this.setState({
        cameraRatio: desiredRatio,
        isCameraRatioSet: true,
        cameraPadding: heightDifference,
        realHeight: realHeight,
        realWidth: screenWidth,
      });
    }
  };
}

const styles = StyleSheet.create({
  appContainer: {
    flex: 1,
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "flex-start",
    paddingVertical: 40,
    paddingHorizontal: 20,
    backgroundColor: DEFAULT_PALETTE.appBackgorund,
  },
  hallChoosingArea: {
    paddingVertical: 40,
    paddingHorizontal: 20,

    display: "flex",
    flexDirection: "row",
    flexWrap: "wrap",
    gap: 10,
  },
  hallChoosingButton: {
    backgroundColor: DEFAULT_PALETTE.button.primary.background,
    borderWidth: 2,
    borderStyle: "solid",
    borderColor: DEFAULT_PALETTE.button.primary.border,
    borderRadius: 5,
    padding: 15,
    fontSize: 20,
  },
  hallChoosingButtonLabel: {
    color: DEFAULT_PALETTE.button.primary.text,
  },
  barCodeScanner: {
    position: "absolute",
  },
});
