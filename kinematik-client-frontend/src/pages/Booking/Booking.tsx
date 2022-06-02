import React, { useState, useEffect } from 'react';
import classes from './Booking.module.css';

import { useParams } from 'react-router-dom';

import axios from 'axios';

import { format } from 'date-fns';
import { uk } from 'date-fns/locale';

import { capitalize, groupBy } from 'lodash';
import classNames from 'classnames';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';

import Button from '../../components/Button/Button';
import { SeatType, widthRegistry } from '../../domain/hallLayout';

import GeneratedTicketDetail from './GeneratedTicketDetail';
import { ReactComponent as SeatIcon } from '../../assets/seat.svg';
import { ReactComponent as CouchIcon } from '../../assets/couch.svg';
import { SeatStatus } from './Booking.';

async function getFilmTitle(filmID: number) {
    const response = await axios.get(`${window.apiBaseUrl}/films/${filmID}/details`);

    return response.data.title;
}

async function getAvailableForBookingSessions(filmID: number) {
    const response = await axios.get(`${window.apiBaseUrl}/sessions/available-for-booking/${filmID}`);

    return response.data.sessions.map((session) => {
        return {
            ...session,
            startAt: new Date(session.startAt),
        };
    });
}

async function getBookingStatuses(sessionID: number) {
    const response = await axios.get(`${window.apiBaseUrl}/bookings/${sessionID}`);

    return response.data.bookingStatuses.map((rawLayoutItem) => {
        return {
            rowID: rawLayoutItem.rowID,
            columnID: rawLayoutItem.columnID,
            seatType: rawLayoutItem.seatTypeID as SeatType,
            seatStatus: rawLayoutItem.isFree ? SeatStatus.FREE : SeatStatus.BOOKED,
        };
    });
}

function splitBookingStatusesIntoLayoutRows(bookingStatuses) {
    const groupedLayoutRows = groupBy(bookingStatuses, (bookingStatus) => bookingStatus.rowID);

    return Object.values(groupedLayoutRows);
}

function computeSessionTimeHierarchy(sessions) {
    return sessions.reduce((accYears, session) => {
        const year = session.startAt.getUTCFullYear();

        if (!accYears[year]) {
            accYears[year] = {};
        }
        const correspondingYear = accYears[year];

        const month = session.startAt.getUTCMonth();
        if (!correspondingYear[month]) {
            correspondingYear[month] = {};
        }
        const correpondingMonth = correspondingYear[month];

        const date = session.startAt.getUTCDate();
        if (!correpondingMonth[date]) {
            correpondingMonth[date] = [];
        }
        const correspondingDate = correpondingMonth[date];

        correspondingDate.push({
            sessionID: session.id,
            startAt: session.startAt,
            hallID: session.hallID,
            hallTitle: session.hallTitle,
        });

        return accYears;
    }, {});
}

function computeAvailableDates(sessionTimeHierarchy) {
    return Object.keys(sessionTimeHierarchy).flatMap((year) => {
        return Object.keys(sessionTimeHierarchy[year]).flatMap((month) => {
            return Object.keys(sessionTimeHierarchy[year][month]).flatMap((date) => {
                return {
                    date: new Date(year, month, date),
                    availableTimes: sessionTimeHierarchy[year][month][date],
                };
            });
        });
    });
}

function getHallItemTypeClasses(seat) {
    const hallItemTypeClasses = [];

    switch (seat.seatType) {
        case SeatType.COMMON:
            hallItemTypeClasses.push(classes['hall-item-common']);
            break;
        case SeatType.VIP:
            hallItemTypeClasses.push(classes['hall-item-vip']);
            break;
        case SeatType.COUCH:
            hallItemTypeClasses.push(classes['hall-item-couch']);
            break;
        case SeatType.EMPTY:
        default:
            hallItemTypeClasses.push(classes['hall-item-empty']);
            break;
    }

    if (seat.seatStatus === SeatStatus.FREE) {
        hallItemTypeClasses.push(classes['hall-item-free']);
    } else if (seat.seatStatus === SeatStatus.SELECTED) {
        hallItemTypeClasses.push(classes['hall-item-selected']);
    } else if (seat.seatStatus === SeatStatus.BOOKED) {
        hallItemTypeClasses.push(classes['hall-item-booked']);
    }

    return classNames(hallItemTypeClasses);
}

function getSeatTypeIcon(seatType: SeatType) {
    switch (seatType) {
        case SeatType.COMMON:
        case SeatType.VIP:
            return <SeatIcon />;
        case SeatType.COUCH:
            return <CouchIcon />;
    }
}

async function bookTicket(sesssionID: number, selectedSeatsIDs: any[]) {
    const request = {
        seatsCoordinates: selectedSeatsIDs.map((selectedSeat) => {
            return {
                rowID: selectedSeat.rowID,
                columnID: selectedSeat.columnID,
            };
        }),
        sessionID: sesssionID,
        clientEmail: 'anton.zhluktarov@gmail.com',
        clientPhone: '+12302032030',
    };
    const response = await axios.put(`${window.apiBaseUrl}/bookings`, request);
}

const Booking = (props) => {
    const params = useParams();

    const [filmTitle, setFilmTitle] = useState();
    const [availableDates, setAvailableDates] = useState([]);
    const [selectedDate, setSelectedDate] = useState(null);
    const [selectedTime, setSelectedTime] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    const [layoutRows, setLayoutRows] = useState([]);
    const [selectedSeats, setSelectedSeats] = useState([]);

    function onSeatClick(seat) {
        let newSelectedItems = null;

        if (seat.seatStatus === SeatStatus.FREE) {
            seat.seatStatus = SeatStatus.SELECTED;
            newSelectedItems = [...selectedSeats, seat];
        } else if (seat.seatStatus !== SeatStatus.BOOKED) {
            seat.seatStatus = SeatStatus.FREE;
            newSelectedItems = selectedSeats.filter((selectedSeat) => selectedSeat !== seat);
        }

        setSelectedSeats(newSelectedItems);
    }

    async function onBookTicketClick() {
        const selectedSeatsIDs = selectedSeats.map((selectedSeat) => {
            return {
                rowID: selectedSeat.rowID,
                columnID: selectedSeat.columnID,
            };
        });

        await bookTicket(selectedTime.sessionID, selectedSeatsIDs);
    }

    useEffect(async () => {
        const fetchedFilmTitle = await getFilmTitle(params.filmId);
        setFilmTitle(fetchedFilmTitle);

        const fetchedAvailableSessions = await getAvailableForBookingSessions(params.filmId);

        const sessionTimeHierarchy = computeSessionTimeHierarchy(fetchedAvailableSessions);
        let computedAvailableDates = computeAvailableDates(sessionTimeHierarchy);
        setAvailableDates(computedAvailableDates);

        if (computedAvailableDates && computedAvailableDates.length > 0) {
            const newSelectedDate = computedAvailableDates[0];
            setSelectedDate(newSelectedDate);

            if (newSelectedDate && newSelectedDate.availableTimes.length > 0) {
                setSelectedTime(newSelectedDate.availableTimes[0]);
            } else {
                setSelectedTime(null);
            }
        } else {
            setSelectedDate(null);
        }

        setIsLoading(false);
    }, []);

    useEffect(async () => {
        if (selectedTime !== null && selectedTime !== undefined) {
            const bookingStatuses = await getBookingStatuses(selectedTime.sessionID);
            const newLayoutRows = splitBookingStatusesIntoLayoutRows(bookingStatuses);

            setLayoutRows(newLayoutRows);
        }
    }, [selectedTime]);

    if (isLoading) {
        return <div>Завантаження...</div>;
    }

    return (
        <div className={classes['booking-layout']}>
            <div className={classes['choosing-area']}>
                <div className={classes['top-row']}>
                    <button className={classes['back-button']}>
                        <FontAwesomeIcon icon={faChevronLeft} />
                    </button>
                    <h1 className={classes['film-title']}>{filmTitle}</h1>
                </div>

                <div className={classes['date-choosing-area']}>
                    {availableDates.map((availableDate) => (
                        <div
                            className={classNames(
                                classes['available-date'],
                                availableDate === selectedDate ? classes['active'] : null
                            )}
                            onClick={() => {
                                setSelectedDate(availableDate);
                            }}
                            key={availableDate.date.getTime()}
                        >
                            {capitalize(
                                format(availableDate.date, 'cccccc, d MMMM', {
                                    locale: uk,
                                })
                            )}
                        </div>
                    ))}
                </div>

                <div className={classes['time-choosing-area']}>
                    {selectedDate.availableTimes.map((availableTime) => (
                        <div
                            className={classNames(
                                classes['available-time'],
                                availableTime === selectedTime ? classes['active'] : null
                            )}
                            onClick={() => {
                                setSelectedTime(selectedTime);
                            }}
                            key={availableTime.startAt.getTime()}
                        >
                            {capitalize(format(availableTime.startAt, 'HH:mm'))}
                        </div>
                    ))}
                </div>

                <div className={classes['seat-choosing-area']}>
                    <div className={classes['hall-layout']}>
                        {layoutRows.map((layoutRow, rowIndex) => (
                            <div className={classes['hall-layout-row']} key={rowIndex}>
                                {layoutRow.map((layoutItem) => (
                                    <div
                                        className={classNames(
                                            classes['hall-layout-item'],
                                            getHallItemTypeClasses(layoutItem, selectedSeats)
                                        )}
                                        style={
                                            {
                                                '--hall-item-relative-width': widthRegistry[layoutItem.seatType],
                                            } as React.CSSProperties
                                        }
                                        key={`${layoutItem.rowID}_${layoutItem.columnID}`}
                                        onClick={
                                            layoutItem.seatStatus !== SeatStatus.BOOKED
                                                ? () => {
                                                      onSeatClick(layoutItem);
                                                  }
                                                : null
                                        }
                                        role={layoutItem.seatStatus !== SeatStatus.BOOKED ? 'button' : null}
                                    >
                                        {getSeatTypeIcon(layoutItem.seatType)}
                                    </div>
                                ))}
                            </div>
                        ))}
                    </div>
                </div>
            </div>

            <div className={classes['generated-ticket']}>
                <div className={classNames(classes['generated-ticket-card'], classes['sidebar-card'])}>
                    <h2 className={classes['sidebar-card-title']}>Квиток</h2>
                    <GeneratedTicketDetail title="Фільм" description={filmTitle} />
                    <GeneratedTicketDetail
                        title="Час"
                        description={
                            capitalize(
                                format(selectedTime.startAt, 'cccccc, d MMMM HH:mm', {
                                    locale: uk,
                                })
                            ) ?? ''
                        }
                    />
                    <GeneratedTicketDetail title="Зала" description={selectedTime.hallTitle} />

                    <GeneratedTicketDetail
                        title="Місця"
                        // TODO Make dynamic
                        description={selectedSeats
                            .map((selectedSeat) => `${selectedSeat.rowID}/${selectedSeat.columnID}`)
                            .join(', ')}
                    />
                </div>
            </div>

            <div className={classes['checkout']}>
                <div className={classNames(classes['checkout-card'], classes['sidebar-card'])}>
                    <h2 className={classes['sidebar-card-title']}>До сплати</h2>
                    Всього: 150 грн
                    <div className={classes['checkout-buttons-container']}>
                        <Button>Купувати квиток</Button>
                        <Button onClick={() => onBookTicketClick()} variant="alternative">
                            Бронювати
                        </Button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Booking;
