import { StyleSheet } from "react-native";

const styles: any = StyleSheet.create({
  container: {
    backgroundColor: "#rgb(41, 37, 36)",
    flex: 1,
    justifyContent: "center",
  },
  content: {
    marginHorizontal: 40,
  },
  title: {
    fontSize: 48,
    fontWeight: "300",
    paddingBottom: 10,
    color: "#fff",
  },
  subtitle: {
    fontSize: 32,
    fontWeight: "800",
    color: "#fff",
  },
  tryAgainButton: {
    backgroundColor: "rgb(226, 30, 32)",
    borderRadius: 50,
    padding: 16,
    marginTop: 40,
  },
  tryAgainButtonText: {
    color: "#fff",
    fontWeight: "600",
    textAlign: "center",
  },
});

export default styles;
