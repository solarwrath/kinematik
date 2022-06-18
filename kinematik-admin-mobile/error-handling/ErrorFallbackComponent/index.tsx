import React, { ReactNode } from "react";
import {
  GestureResponderEvent,
  SafeAreaView,
  Text,
  TouchableOpacity,
  View,
} from "react-native";

import styles from "./styles";

const ErrorFallbackComponent = (props: {
  error: Error;
  resetError?: (event: GestureResponderEvent) => void;
}): ReactNode => {
  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.content}>
        <Text style={styles.title}>Упс!</Text>
        <Text style={styles.subtitle}>Виникла помилка</Text>
        <TouchableOpacity
          style={styles.tryAgainButton}
          onPress={props.resetError}
        >
          <Text style={styles.tryAgainButtonText}>Спробувати ще раз</Text>
        </TouchableOpacity>
      </View>
    </SafeAreaView>
  );
};

export default ErrorFallbackComponent;
